using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums;

public enum AuthActionEventType
{
    RegisterSuccess,
    RegisterFailed,
    LoginSuccess,
    LoginFailed,
    AccountLocked,
    AccountUnlocked,
    AccountLockedAttempt,
    PasswordChanged,
    RefreshTokenIssued,
    RefreshTokenRevoked,
    Logout
}
