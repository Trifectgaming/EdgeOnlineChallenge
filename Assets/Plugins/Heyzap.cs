//
//  Heyzap.cs
//
//  Copyright 2013 Smart Balloon, Inc. All Rights Reserved
//
//  Permission is hereby granted, free of charge, to any person
//  obtaining a copy of this software and associated documentation
//  files (the "Software"), to deal in the Software without
//  restriction, including without limitation the rights to use,
//  copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the
//  Software is furnished to do so, subject to the following
//  conditions:
//
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//  OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
//  HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//  OTHER DEALINGS IN THE SOFTWARE.
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public class Heyzap : MonoBehaviour {
	public delegate void LevelListener( string levelId );
  private static LevelListener levelRequestListener;

  public static int FLAG_NO_OPTIONS = 1 << 0;
  public static int FLAG_NO_HEYZAP_INSTALL_SPLASH = 1 << 1;
  public static int FLAG_NO_NOTIFICATION = 1 << 23;
  public static int FLAG_SUBTLE_NOTIFICATION = 1 << 24;
  public static int FLAG_MINIMAL_ACHIEVEMENT_DIALOG = 1 << 25;

  private static string pendingLevelId;
  private static Heyzap _instance = null;

  #if UNITY_ANDROID
  public static void start(int options){
      HeyzapAndroid.start(options);
      Heyzap.initReceiver();
  }
  #endif
  public static void start(int options, int appStoreId) {
      #if UNITY_ANDROID
      HeyzapAndroid.start(options);
      #endif

      #if UNITY_IPHONE
      if (appStoreId <= 0) {
        throw new System.ArgumentException("App Store ID must be greater than 0.", "appStoreId");
      }

      HeyzapIOS.start(appStoreId, "", options);
      #endif

      Heyzap.initReceiver();
  }

  public static void checkin(string message) {
		#if UNITY_ANDROID
		HeyzapAndroid.checkin(message);
		#endif

    #if UNITY_IPHONE
    HeyzapIOS.checkin(message);
    #endif
	}
	
	public static void checkin() {
		Heyzap.checkin ("");
	}

  public static Boolean isSupported() {
      return true;
  }
	
	public static void setFlags (int flags) {
		#if UNITY_ANDROID
		HeyzapAndroid.setFlags(flags);
		#endif
	}
	
	public static void submitScore(int relativeScore, string displayScore, string levelId) {
		#if UNITY_ANDROID
		HeyzapAndroid.submitScore(relativeScore, displayScore, levelId);
		#endif

    #if UNITY_IPHONE
    HeyzapIOS.submitScore(relativeScore, displayScore, levelId);
    #endif
	}
	
	public static void showLeaderboards(string levelId) {
		#if UNITY_ANDROID
		HeyzapAndroid.showLeaderboards(levelId);
		#endif

    #if UNITY_IPHONE
    HeyzapIOS.showLeaderboards(levelId);
    #endif
	}

  public static void showLeaderboards(){
    showLeaderboards("");
  }
	
	public static void showAchievements() {
		#if UNITY_ANDROID
		HeyzapAndroid.showAchievements();	
		#endif

    #if UNITY_IPHONE
    HeyzapIOS.showAchievements(); 
    #endif
	}
	
	public static void unlockAchievements(string[] achievementIds) {
		#if UNITY_ANDROID
		HeyzapAndroid.unlockAchievements(achievementIds);
		#endif

    #if UNITY_IPHONE
    HeyzapIOS.unlockAchievements(achievementIds); 
    #endif    
	}
	
	public static void setLevelRequestListener(LevelListener listener) {
    levelRequestListener = listener;
		if(pendingLevelId != null){
        	requestLevelS(pendingLevelId);
		}
  }
 
	public void requestLevel(string levelId) { requestLevelS(levelId); }
	public static void requestLevelS(string levelId) {
        if(levelRequestListener != null){
            levelRequestListener(levelId);
            pendingLevelId = null;
        }else{
            pendingLevelId = levelId;
        }
    }
	
	public static void initReceiver(){
		if (_instance == null) {
			GameObject receiverObject = new GameObject("Heyzap");
			DontDestroyOnLoad(receiverObject);
			_instance = receiverObject.AddComponent<Heyzap>();
		}
	}
}

#if UNITY_IPHONE
public class HeyzapIOS : MonoBehaviour {

	/* Start */
  public static void start(int appStoreId, string urlSchema, int options) {
		hz_start(appStoreId, urlSchema, options);
	}

  [DllImport ("__Internal")]
  private static extern void hz_start(int appId, string urlSchema, int flags);

  /* Submit Score */
  public static void submitScore(int relativeScore, string displayScore, string levelId) {
    hz_submit_score(relativeScore.ToString(), displayScore, levelId);
  }

  [DllImport ("__Internal")]
  private static extern void hz_submit_score(string realScore, string displayScore,  string levelId);

  /* Show Leaderboards */
  public static void showLeaderboards(string levelId = "") {
    hz_show_leaderboard(levelId);
  }

  [DllImport ("__Internal")]
  private static extern void hz_show_leaderboard(string levelId);

  /* Show Achievements */
  public static void showAchievements() {
    hz_show_achievements();
  }

  [DllImport ("__Internal")]
  private static extern void hz_show_achievements();

    /* Show Leaderboards */
  public static void unlockAchievements(string[] achievementIds) {
    String achievementIdsStr = string.Join(",", achievementIds);
    hz_unlock_achievements(achievementIdsStr);
  }

  [DllImport ("__Internal")]
  private static extern void hz_unlock_achievements(string achievementIDs);

  /* Checkin */
  public static void checkin(string message = "") {
    hz_checkin(message);
  }

  [DllImport ("__Internal")]
  private static extern void hz_checkin(string message);

}
#endif

#if UNITY_ANDROID
public class HeyzapAndroid : MonoBehaviour {
	public static void start(int options = 0) {
	  AndroidJNIHelper.debug = true; 
      using (AndroidJavaClass jc = new AndroidJavaClass("com.heyzap.sdk.extensions.UnityHelper")) { 
        jc.CallStatic("start", options);
      }
    }
	
	public static void setFlags(int flags) {
	  AndroidJNIHelper.debug = true; 
      using (AndroidJavaClass jc = new AndroidJavaClass("com.heyzap.sdk.extensions.UnityHelper")) { 
        jc.CallStatic("setFlags", flags);
      }
	}
	
	public static void checkin(string message = "") {
	  AndroidJNIHelper.debug = true; 
      using (AndroidJavaClass jc = new AndroidJavaClass("com.heyzap.sdk.extensions.UnityHelper")) { 
        jc.CallStatic("checkin", message);
      }
    }
	
	public static void submitScore(int relativeScore, string displayScore, string levelId) {
	  AndroidJNIHelper.debug = true; 
      using (AndroidJavaClass jc = new AndroidJavaClass("com.heyzap.sdk.extensions.UnityHelper")) { 
        jc.CallStatic("submitScore", relativeScore.ToString(), displayScore, levelId);
      }
    }
	
	public static void showLeaderboards(string levelId = null) {
		AndroidJNIHelper.debug = true; 
	    using (AndroidJavaClass jc = new AndroidJavaClass("com.heyzap.sdk.extensions.UnityHelper")) { 
			if (levelId == null) {
        		jc.CallStatic("showLeaderboards");
			} else {
        		jc.CallStatic("showLeaderboards", levelId);
			}
		}
    }
	
	public static void showAchievements() {
		AndroidJNIHelper.debug = true; 
      	using (AndroidJavaClass jc = new AndroidJavaClass("com.heyzap.sdk.extensions.UnityHelper")) { 
        	jc.CallStatic("showAchievements");
      	}		
	}
	
	public static void unlockAchievements(string[] achievementIds) {
		String achievementIdsStr = string.Join(",", achievementIds);
		AndroidJNIHelper.debug = true; 
      	using (AndroidJavaClass jc = new AndroidJavaClass("com.heyzap.sdk.extensions.UnityHelper")) { 
        	jc.CallStatic("unlockAchievements", achievementIdsStr);
      	}
	}
}
#endif
