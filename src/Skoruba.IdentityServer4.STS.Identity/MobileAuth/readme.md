# IdentityServer4.PhoneNumberAuth
Sample passwordless phone number authentication with IdentityServer4.

> NOTE: To be able to test locally you can change `"ReturnVerifyTokenForTesting : true"` on `appsettings.json` it will returns us `verification_token` on response, however in production usages it must be removed and you should add real SMS service (Twilio, Nexmo, etc..) by implementing `ISmsServices`

```console
curl -H "Content-Type: application/json" \ 
     -X POST \
     -d '{
        "phonenumber":"+1234567890",
        "AppHash":"A!@#$DSAFA",
        "DeviceId":"abcdefghijklmnopqrstuvwxyz",
        "NotificationId" : "abcdefghijklmnopqrstuvwxyz0123456789"
        }' \ 
     http://localhost:44310/api/mobile/generate-otp 
```
```json
{
    "protect_token": "CfDJ8F2fHxOfr9xAtc...",
    "verification_token": "373635"
}
```

Authentication by verification token

```console
curl -H "Content-Type: application/x-www-form-urlencoded" \
     -X POST \ 
     -d grant_type=phone_number_token&client_id=phone_number_authentication&client_secret=secret&phone_number=+198989822&verification_token=373635 \ 
      


client_id: "phone_number_authentication"
client_secret: "secret"
grant_type: "phone_number"
phone_number: "+123456789"
protect_token: "CfDJ8F2fHxOfr9xAtc..."
verification_token: "373635"
acr_values: "device_id:abcdefghijklmnopqrstuvwxyz notification_id:abcdefghijklmnopqrstuvwxyz0123456789"


http://localhost:44310/connect/token
```

```json
{
    "access_token": "CfDJ8F2fHxOfr9xAtc......",
    "expires_in": 3600,
    "token_type": "Bearer",
    "refresh_token": "CfDJ8F2fHxOfr9xAtc...."
}
```
