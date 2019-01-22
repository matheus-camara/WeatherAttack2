﻿
using Valit;
using WeatherAttack.Domain.EntityValidation.Rules;
using WeatherAttack.Domain.Notifications;

namespace WeatherAttack.Domain.Entities
{
    public class User : EntityBase
    {
        public string Email { get; private set; }

        public string Username { get; private set; }

        public string Password { get; private set; }

        public Character Character { get; private set; } = new Character();

        public User(string email, string username)
        {
            Email = email;
            Username = username;
        }

        public void SetPassword(string password)
        {
            Password = password;
        }

        protected override void Validate()
        {
            var result = ValitRules<User>
                .Create()
                .Ensure(u => u.Email, _ => _
                    .Required()
                        .WithMessage(WeatherAttackNotifications.User.EmailIsRequired)
                    .Satisfies(u => u.Trim().Length > Rules.User.Email.MinLength)
                        .WithMessage(WeatherAttackNotifications.User.InvalidEmail)
                    .Email()
                        .WithMessage(WeatherAttackNotifications.User.InvalidEmail)
                    .MaxLength(Rules.User.Email.MaxLength)
                        .WithMessage(WeatherAttackNotifications.User.InvalidEmail))
                 .Ensure(u => u.Username, _ => _
                    .Required()
                        .WithMessage(WeatherAttackNotifications.User.UsernameIsRequired)
                    .Satisfies(u => u.Trim().Length > Rules.User.Username.MinLength)
                        .WithMessage(WeatherAttackNotifications.User.InvalidUsername)
                    .MaxLength(Rules.User.Username.MaxLength)
                        .WithMessage(WeatherAttackNotifications.User.InvalidUsername))
                .Ensure(u => u.Password, _ => _
                    .Required()
                        .WithMessage(WeatherAttackNotifications.User.PasswordIsRequired))
                .For(this)
                .Validate();

            AddNotification(WeatherAttackNotifications.GetNotification(result.ErrorMessages));
        }
    }
}