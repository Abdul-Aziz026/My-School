using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums;

public enum AuthEventType
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
