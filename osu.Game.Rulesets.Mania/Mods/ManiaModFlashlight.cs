﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Layout;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mania.Objects;
using osu.Game.Rulesets.Mods;
using osuTK;

namespace osu.Game.Rulesets.Mania.Mods
{
    public class ManiaModFlashlight : ModFlashlight<ManiaHitObject>
    {
        public override double ScoreMultiplier => 1;
        public override Type[] IncompatibleMods => new[] { typeof(ModHidden) };

        [SettingSource("Change radius based on combo", "Decrease the flashlight radius as combo increases.")]
        public override BindableBool ChangeRadius { get; } = new BindableBool
        {
            Default = false,
            Value = false
        };

        [SettingSource("Initial radius", "Initial radius of the flashlight area.")]
        public override BindableNumber<float> InitialRadius { get; } = new BindableNumber<float>
        {
            MinValue = 0f,
            MaxValue = 4.5f,
            Default = 1f,
            Value = 1f,
            Precision = 0.1f
        };

        protected override float ModeMultiplier => 50;

        public override Flashlight CreateFlashlight() => new ManiaFlashlight(ChangeRadius.Value, InitialRadius.Value, ModeMultiplier);

        private class ManiaFlashlight : Flashlight
        {
            private readonly LayoutValue flashlightProperties = new LayoutValue(Invalidation.DrawSize);

            public ManiaFlashlight(bool isRadiusBasedOnCombo, float initialRadius, float modeMultiplier)
                : base(isRadiusBasedOnCombo, initialRadius, modeMultiplier)
            {
                FlashlightSize = new Vector2(DrawWidth, GetRadiusFor(0));

                AddLayout(flashlightProperties);
            }

            protected override void Update()
            {
                base.Update();

                if (!flashlightProperties.IsValid)
                {
                    FlashlightSize = new Vector2(DrawWidth, FlashlightSize.Y);

                    FlashlightPosition = DrawPosition + DrawSize / 2;
                    flashlightProperties.Validate();
                }
            }

            protected override void OnComboChange(ValueChangedEvent<int> e)
            {
                this.TransformTo(nameof(FlashlightSize), new Vector2(DrawWidth, GetRadiusFor(e.NewValue)), FLASHLIGHT_FADE_DURATION);
            }

            protected override string FragmentShader => "RectangularFlashlight";
        }
    }
}
