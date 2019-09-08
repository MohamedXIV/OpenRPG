using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using Facebook.Unity;
using LoginResult = PlayFab.ClientModels.LoginResult;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System;

public class PlayFabLogin : MonoBehaviour
{
    public Text userEmail, userPassword, username;
    //private string authTicket;
    private GetPlayerCombinedInfoRequestParams InfoRequestParams;
    public GameObject navbar;
    public GameObject notValidEmailText;
    public InputField emailInputField;
    public InputField passwordInputField;
    public Button signInBtn;
    public Button registerInBtn;
    private bool rememberMe, loginOnce;
    private string _message;
    public Animator loginPanelAnimator;
    public Animator loginBgAnimator;
    private bool validEmail;
    private bool validPassword;
    private const string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
     + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
     + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
     + @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";
    private const string MatchPasswordPattern = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{4,8}$";

    // Start is called before the first frame update
    private void Start()
    {
        CheckExistingEmail();
        //Debug.Log(PlayerPrefs.GetString("ACCESSTOKEN"));
    }
    private void Update()
    {
        ToggleInteractSignInBtn();
    }

    public void LoginWithEmail()
    {
        if(validEmail && validPassword)
        {
            var request = new LoginWithEmailAddressRequest() { Email = userEmail.text, Password = userPassword.text };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }
    }
    
    public void LoginWithEmail(string email, string password)
    {
        var request = new LoginWithEmailAddressRequest() {Email = email, Password = password};
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    public void RegisterWithEmail()
    {
        var registerRequest = new RegisterPlayFabUserRequest() {Email = userEmail.text, Password = userPassword.text, Username = username.text};
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
    }

    public void AddEmailPassword()
    {
        var addUserRequest = new AddUsernamePasswordRequest() { Email = userEmail.text, Password = userPassword.text, Username = username.text };
        PlayFabClientAPI.AddUsernamePassword(addUserRequest, OnAddUserSuccess, OnRegisterFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you have been logged in successfully!");
        if (validEmail && rememberMe)
        {
            PlayerPrefs.SetString("EMAIL", userEmail.text);
            PlayerPrefs.SetString("PASSWORD", userPassword.text);
            PlayerPrefs.SetInt("LOGINONCE", 1);
        }
        LoadSceneOnLoginSuccess();
    }
    
    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Congratulations, new user have been created successfully!");
        if (validEmail && rememberMe)
        {
            PlayerPrefs.SetString("EMAIL", userEmail.text);
            PlayerPrefs.SetString("PASSWORD", userPassword.text);
            PlayerPrefs.SetInt("LOGINONCE", 1);
        }
        LoadSceneOnLoginSuccess();
    }

    private void OnAddUserSuccess(AddUsernamePasswordResult result)
    {
        Debug.Log("Congratulations, new user have been created successfully!");
        if (validEmail && rememberMe)
        {
            PlayerPrefs.SetString("EMAIL", userEmail.text);
            PlayerPrefs.SetString("PASSWORD", userPassword.text);
            PlayerPrefs.SetInt("LOGINONCE", 1);
        }
        LoadSceneOnLoginSuccess();
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogWarning("Maybe you need to register first");
        //PlayFabClientAPI.RegisterPlayFabUser();
    }
    
    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError(error.GenerateErrorReport());
    }

    private void CheckExistingEmail()
    {
        loginOnce = Convert.ToBoolean(PlayerPrefs.GetInt("LOGINONCE"));
        if (PlayerPrefs.HasKey("EMAIL"))
        {
            LoginWithEmail(PlayerPrefs.GetString("EMAIL"), PlayerPrefs.GetString("PASSWORD"));
            Debug.Log("We logged in with email");
        }
        else
        {
            LoginWithAndroid();
            Debug.Log("We logged in with Android device ID");
        }
    }

    #region Facebook Login

    private void OnFacebookInitialized()
    {
        SetMessage("Logging into Facebook...");

        // Once Facebook SDK is initialized, if we are logged in, we log out to demonstrate the entire authentication cycle.
        if (FB.IsLoggedIn)
        {
            Debug.Log("We have been loggedin previously");
        }
        //CheckHasAccessToken();
        //    FB.LogOut();

        // We invoke basic login procedure and pass in the callback to process the result
        FB.LogInWithReadPermissions(null, OnFacebookLoggedIn);
    }

    private void CheckHasAccessToken()
    {
        if (PlayerPrefs.HasKey("ACCESSTOKEN"))
        {
            PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest
            {
                TitleId = PlayerPrefs.GetString("TITLEID"),
                CreateAccount = false,
                AccessToken = PlayerPrefs.GetString("ACCESSTOKEN")

            },
             OnPlayfabFacebookAuthComplete, OnPlayfabFacebookAuthFailed);
            Debug.Log("We went here!");
        }
    }

    private void OnFacebookLoggedIn(ILoginResult result)
    {
        // If result has no errors, it means we have authenticated in Facebook successfully
        if (result == null || string.IsNullOrEmpty(result.Error))
        {
            SetMessage("Facebook Auth Complete! Access Token: " + AccessToken.CurrentAccessToken.TokenString + "\nLogging into PlayFab...");

            /*
             * We proceed with making a call to PlayFab API. We pass in current Facebook AccessToken and let it create 
             * and account using CreateAccount flag set to true. We also pass the callback for Success and Failure results
             */
            PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest
            {
                TitleId = PlayFabSettings.TitleId,
                CreateAccount = true,
                AccessToken = AccessToken.CurrentAccessToken.TokenString,
                InfoRequestParameters = InfoRequestParams
            },
                OnPlayfabFacebookAuthComplete, OnPlayfabFacebookAuthFailed);
            
        }
        else
        {
            // If Facebook authentication failed, we stop the cycle with the message
            SetMessage("Facebook Auth Failed: " + result.Error + "\n" + result.RawResult, true);
        }
    }
    
    // When processing both results, we just set the message, explaining what's going on.
    private void OnPlayfabFacebookAuthComplete(LoginResult result)
    {
        SetMessage("PlayFab Facebook Auth Complete. Session ticket: " + result.SessionTicket);
        if (rememberMe)
        {
            SetPlayerPrefs(PlayFabSettings.TitleId, AccessToken.CurrentAccessToken.TokenString);
        }
        LoadSceneOnLoginSuccess();
    }
    
    private void OnPlayfabFacebookAuthFailed(PlayFabError error)
    {
        SetMessage("PlayFab Facebook Auth Failed: " + error.GenerateErrorReport(), true);
    }
    
    public void SetMessage(string message, bool error = false)
    {
        _message = message;
        if (error)
            Debug.LogError(_message);
        else
            Debug.Log(_message);
    }
    
    public void OnGUI()
    {
        var style = new GUIStyle { fontSize = 40, normal = new GUIStyleState { textColor = Color.white }, alignment = TextAnchor.MiddleCenter, wordWrap = true };
        var area = new Rect(0,0,Screen.width,Screen.height);
        GUI.Label(area, _message,style);
    }
    
    public void FacebookInitializing()
    {
        SetMessage("Initializing Facebook..."); // logs the given message and displays it on the screen using OnGUI method

        // This call is required before any other calls to the Facebook API. We pass in the callback to be invoked once initialization is finished
        FB.Init(OnFacebookInitialized);    
    }

    #endregion

    #region Android Login

#if UNITY_ANDROID
    public void LoginWithAndroid()
    {
        var requestAndroid = new LoginWithAndroidDeviceIDRequest
        {
            AndroidDeviceId = ReturnDeviceId(),
            CreateAccount = true,
        };
        PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid, OnAndroidLoginSuccess, OnAndroidLoginFailure);
        
    }

    private static string ReturnDeviceId()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    private void OnAndroidLoginSuccess(LoginResult result)
    {
        Debug.Log("you have been logged in successfully!");
        if (loginOnce)
        {
            LoadSceneOnLoginSuccess();
        }
    }

    private void OnAndroidLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call.  :(");
        Debug.LogError(error.GenerateErrorReport());
    }

#endif
    #endregion

    private void LoadSceneOnLoginSuccess()
    {
        SceneManager.LoadScene("Level1", LoadSceneMode.Additive);
        ShowNavbar();
        loginPanelAnimator.SetBool("isShown", false);
        loginBgAnimator.SetBool("isShown", false);
        
    }

    public void ShowNavbar()
    {
        navbar.SetActive(true);
    }

    public void TestToken()
    {
        Debug.Log(AccessToken.CurrentAccessToken != null);
    }

    public void SetPlayerPrefs(string titleId, string accessToken)
    {
        PlayerPrefs.SetString("TITLEID", titleId);
        PlayerPrefs.SetString("ACCESSTOKEN", accessToken);
    }
    public string GetPlayerPrefs(string titleId)
    {
        return PlayerPrefs.GetString(titleId);
    }

    public void ToggleRemember(bool newValue)
    {
        rememberMe = newValue;
    }

    //Check Email validation
    public void OnValueChangedEmail(string emailInput)
    {
        if(!string.IsNullOrEmpty(emailInput) && Regex.IsMatch(emailInput, MatchEmailPattern))
        {
            emailInputField.image.color = Color.green;
            Debug.Log(userEmail.text);
            notValidEmailText.SetActive(false);
            validEmail = true;
        }
        else
        {
            emailInputField.image.color = Color.red;
            notValidEmailText.SetActive(true);
            validEmail = false;
        }
    }

    //Check Password validation
    public void OnValueChangedPassword(string passwordInput)
    {
        if (!string.IsNullOrEmpty(passwordInput) && Regex.IsMatch(passwordInput, MatchPasswordPattern))
        {
            passwordInputField.image.color = Color.green;
            notValidEmailText.SetActive(false);
            validPassword = true;
        }
        else
        {
            passwordInputField.image.color = Color.red;
            notValidEmailText.SetActive(true);
            validPassword = false;
        }
    }

    //Toggle interactable sign in buttron
    public void ToggleInteractSignInBtn()
    {
        if(validEmail && validPassword)
        {
            signInBtn.interactable = true;
        }
        else
        {
            signInBtn.interactable = false;
        }
    }
}
