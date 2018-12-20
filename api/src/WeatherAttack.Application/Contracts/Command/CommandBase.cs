﻿using System;
using System.Collections.Generic;
using System.Text;
using WeatherAttack.Domain.Entities;

namespace WeatherAttack.Application.Contracts.Command
{
    public class CommandBase : EntityBase, ICommand
    {
        public virtual void Execute()
        {
            throw new NotImplementedException();
        }
    }
}