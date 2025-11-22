# Generate token

- Navigate to https://developer.spotify.com/ in order to generate your credentials.
- Open the application and modify the appsettings or add secrets for the following fields:
{ 
  "Spotify:ClientSecret": "*",
  "Spotify:ClientId": "*",
  "Kestrel:Certificates:Development:Password": "*",
  "Jwt:SigningKey": "*"
}


# Authorizing your user

- Run the app using docker compose.
- Navigate to: https://localhost:7002/api/Auth/authorize or the desired port of your choosing if you decide to set up another.
- This will call the **Authorize Endpoint** which will generate a Redirect URI using your supplied ClientId on step 1.
- After the URI is generated the app will redirect you to spotify and you will be asked to login. This will interact with the **HandleSpotifyCallback Endpoint** After login you'll get something like this:

```
{"access_token":"yourLongAccesToken","token_type":"Bearer","scope":"user-read-currently-playing user-top-read","expires_in":3600,"refresh_token":"yourLongRefreshToken"}
```

So, the full OAuth flow is:

**1**. Redirect to Spotify → ✔

**2**. User logs in → ✔

**3**. Spotify redirects back with code → ✔

**4**. API exchanges code for tokens → ✔

**5**. API outputs token JSON → ✔

- Tokens will be cached in redis.
