using System;
using Discord;
using UniRx;
using UnityEngine;

namespace CDK.Integration {
	public static class CDiscordController {

		private static Discord.Discord discord;
		private static CompositeDisposable _compositeDisposable;
		private static long _epochStartTime;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		public static void Init() {
			_compositeDisposable?.Dispose();
			discord?.Dispose();

			long clientId = 0;
			try {
				clientId = Convert.ToInt64(CGameSettings.get.DiscordClientId);
			} catch (Exception e) {
				Debug.LogError(e);
			}
			if (clientId == 0) return;
			
			discord = new Discord.Discord(clientId, (UInt64)Discord.CreateFlags.NoRequireDiscord);
			if (discord == null) {
				Debug.LogError($"Error creating Discord Controller and instance now is null.");
				return;
			}
			Debug.Log("Created Discord Controller");

			_epochStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
			
			_compositeDisposable = new CompositeDisposable();
			
			_compositeDisposable.Add(
			Observable.Timer(TimeSpan.FromSeconds(5)).RepeatSafe().Subscribe(_ => {
				if (discord == null) return;
				discord.RunCallbacks();
			})
			);
			
			SetRichPresence(States.playing);
		}


		public enum States {
			onMenu, wakeup, dreaming, playing
		}


		public static void SetRichPresence(States newState) {
			if (discord == null) return;
			
			var activityManager = discord.GetActivityManager();

			var stateText = newState.ToString().CFirstLetterToUpperCase();
			
			var activity = new Discord.Activity {
				State = stateText,
				Timestamps = {Start = _epochStartTime},
				Assets = new ActivityAssets {
					LargeImage = "roomsquare"
				}
			};
			activityManager.UpdateActivity(activity, (res) => {
				if (res == Discord.Result.Ok) {
					Debug.Log($"Discord Activity updated to state '{stateText}'.");
				}
				else {
					Debug.LogWarning($"Issue updating user Discord Activity:\n{res}");
				}
			});
		}
		
	}
}
