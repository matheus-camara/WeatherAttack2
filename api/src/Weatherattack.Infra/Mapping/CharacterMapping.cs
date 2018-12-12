﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Weatherattack.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weatherattack.Domain.EntityValidation.Rules.Character;

namespace Weatherattack.Infra.Mapping
{
    class CharacterMapping : IEntityTypeConfiguration<Character>
    {
        public void Configure(EntityTypeBuilder<Character> builder)
        {
            builder.ToTable("Character");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Battles)
                .HasDefaultValue(CharacterRules.Battles.InitialValue);
            builder.Property(c => c.Wins)
                .HasDefaultValue(CharacterRules.Wins.InitialValue);
            builder.Property(c => c.Losses)
                .HasDefaultValue(CharacterRules.Losses.InitialValue);
            builder.Property(c => c.Medals)
                .HasDefaultValue(CharacterRules.Medals.InitialValue);
        }
    }
}
