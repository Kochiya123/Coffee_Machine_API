using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

public class FirebaseService
{
    private readonly FirebaseAuth _auth;

    public async Task<string?> VerifyFirebaseToken(string idToken)
    {
        try
        {
            var decodedToken = await _auth.VerifyIdTokenAsync(idToken);
            return decodedToken.Claims["email"]?.ToString(); // Extract email from the token
        }
        catch
        {
            return null; // Invalid token
        }
    }
}
