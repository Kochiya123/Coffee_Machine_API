using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

public class FirebaseService
{
    public FirebaseService()
    {
        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("path/to/your/firebase-service-account.json")
            });
        }
    }

    public async Task<string> VerifyIdTokenAsync(string idToken)
    {
        try
        {
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
            return decodedToken.Uid; // Return the user's UID
        }
        catch (Exception ex)
        {
            throw new Exception("Invalid ID token", ex);
        }
    }
}