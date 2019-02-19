﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using WeatherAttack.Domain.Notifications;

namespace WeatherAttack.Domain.Entities
{
    public class EntityBase
    {
        protected EntityBase() { }

        public EntityBase(long id) => Id = id;

        public long Id { get; private set; }

        public bool IsNew => Id == 0;

        public bool IsValid => Validate();

        [NotMapped]
        public List<Notification> Notifications { get; private set; } = new List<Notification>();

        public void AddNotification(string code)
        {
            AddNotification(WeatherAttackNotifications.Get(code));
        }

        public void AddNotification(ImmutableArray<string> codeArray)
        {
            AddNotification(WeatherAttackNotifications.Get(codeArray));
        }

        public void AddNotification(Notification notification)
        {
            if(!Notifications.Contains(notification))
                Notifications.Add(notification);
        }

        public void AddNotification(List<Notification> notifications)
        {
            if(notifications.Count > 0)
                notifications.ForEach(n => AddNotification(n));
        }

        protected virtual bool Validate()
        {
            return Notifications.Count == 0;
        }
    }
}