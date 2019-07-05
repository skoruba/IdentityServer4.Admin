using Skoruba.IdentityServer4.Audit.Sink.Adapters;
using IdentityServer4.Events;
using Skoruba.IdentityServer4.Audit.Sink.Events;
using Microsoft.AspNetCore.Http;

namespace Skoruba.IdentityServer4.Audit.Sink
{
    public class AdapterFactory : IAdapterFactory
    {
        public IAuditArgs Create(Event evt, IHttpContextAccessor httpContext)
        {
            if (evt != null)
            {
                switch (evt)
                {
                    case TokenIssuedSuccessEvent tokenIssuedSuccessEvent:
                        return new TokenIssuedSuccessEventAdapter(tokenIssuedSuccessEvent, httpContext);

                    case UserLoginSuccessEvent userLoginSuccess:
                        return new UserLoginSuccessEventAdapter(userLoginSuccess, httpContext);

                    case UserLoginFailureEvent userLoginFailure:
                        return new UserLoginFailureEventAdapter(userLoginFailure, httpContext);

                    case UserLogoutSuccessEvent userLogoutSuccess:
                        return new UserLogoutSuccessEventAdapter(userLogoutSuccess, httpContext);

                    case ConsentGrantedEvent consentGranted:
                        return new ConsentGrantedEventAdapter(consentGranted, httpContext);

                    case ConsentDeniedEvent consentDenied:
                        return new ConsentDeniedEventAdapter(consentDenied, httpContext);

                    case TokenIssuedFailureEvent tokenIssuedFailure:
                        return new TokenIssuedFailureEventAdapter(tokenIssuedFailure, httpContext);

                    case GrantsRevokedEvent grantsRevoked:
                        return new GrantsRevokedEventAdapter(grantsRevoked, httpContext);

                    case ClientChangedEvent clientChangedEvent:
                        return new ClientChangedEventAdapter(clientChangedEvent, httpContext);

                    case PasswordChangedEvent passwordChangedEvent:
                        return new PasswordChangedEventAdapter(passwordChangedEvent, httpContext);
                }
            }

            return null;
        }
    }
}