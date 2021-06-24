using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRC;
using MelonLoader;
using UnityEngine;
using Harmony;
using System.Reflection;
using System.Collections;

namespace Client_Detector
{
	public class ClientSeek : MelonMod
	{
		public override void OnApplicationStart()
        {
			ClientSeek.CheckPlayers().Start();
		}
		public static IEnumerator CheckPlayers()
        {
			for(; ; )
            {
				if(PlayerExtensions.IsInWorld())
                {
					yield return QueuePlayerActions(delegate (Player player)
					{
                        try
                        {
							DetectUser(player);
						}
                        catch
						{
                        }
					}, 0.5f);
				}
                else
                {
					yield return new WaitForSeconds(4);
                }
			}
			yield break;

		}
		public static IEnumerator QueuePlayerActions(Action<Player> OnPlayerAction, float WaitBetweenPlayer)
		{
			var AllPlayers = PlayerExtensions.GetAllPlayers();
			foreach (var player in AllPlayers)
			{
				if(player != null)
                {
					OnPlayerAction?.Invoke(player);
					yield return new WaitForSeconds(WaitBetweenPlayer);
				}
			}
			yield return null;
		}
		public static void InitPatches()
        {
			ClientSeek.InitOnPlayerJoinLeavePatch();
		}
		public static void InitOnPlayerJoinLeavePatch()
		{
			MethodInfo[] array = (from m in typeof(NetworkManager).GetMethods()
								  where m.Name.Contains("Method_Public_Void_Player_") && !m.Name.Contains("PDM")
								  select m).ToArray<MethodInfo>();
			new Patch(typeof(NetworkManager), typeof(ClientSeek), array[0].Name, "OnPlayerLeft", BindingFlags.Static, BindingFlags.NonPublic);
			new Patch(typeof(NetworkManager), typeof(ClientSeek), array[1].Name, "OnPlayerJoined", BindingFlags.Static, BindingFlags.NonPublic);
			new Patch(typeof(NetworkManager), typeof(ClientSeek), "Method_Public_Virtual_Final_New_Void_EventData_0", "OnEvent", BindingFlags.Static, BindingFlags.NonPublic);
		}
		private static void OnPlayerLeft(ref Player __0)
        {
			
		}

		public static ClientSeek.ClientDetectionLevel GetClientDetectionLevel(Player player)
		{
			return ClientSeek.DetectUser(player);
		}

		private static ClientSeek.ClientDetectionLevel DetectUser(Player player)
		{
			if (player.GetPlayerPing() < 0 || (player.GetPlayerFrames() < 10 && player.GetPlayerFrames() != 0) || ClientSeek.PlayerIsSpoofingQuest(player) || (player.GetPlayerFrames() > 200 || player.GetPlayerPing() > 1000 && !player.IsQuest()))
			{
				if (Nameplates.ContainsKey(PlayerExtensions.GetAPIUser(player).id))
				{
					if(Nameplates[player.GetAPIUser().id].gameObject == null)
                    {
						Nameplates.Remove(player.GetAPIUser().id);
                    }
				}
				else
				{
					Nameplates.Add(PlayerExtensions.GetAPIUser(player).id, new NameplateText(player.GetVRCPlayer(), "Client User", new Vector2( 0f, 100f), Color.red));
					MelonLogger.Msg($"{player.GetName()} has been detected");
				}
				return ClientSeek.ClientDetectionLevel.Using;
			}
			if (player.GetPlayerPing() > 1000 || player.GetPlayerFrames() > 144)
				{
					return ClientSeek.ClientDetectionLevel.Likely_Using;
				}
				return ClientSeek.ClientDetectionLevel.Not_Using;
			}

			private static bool IsFlying(Player player)
			{
				return player.GetVRCPlayerApi().GetVelocity().y > 10f && player.GetVRCPlayerApi().GetVelocity().y < -10f && !player.GetVRCPlayerApi().IsPlayerGrounded();
			}

			private static bool PlayerIsSpeedHacking(Player player)
			{
				return player.GetVRCPlayerApi().GetWalkSpeed() != 2f || player.GetVRCPlayerApi().GetRunSpeed() != 4f || player.GetVRCPlayerApi().GetStrafeSpeed() != 2f;
			}

			private static bool PlayerIsSpoofingQuest(Player player)
			{
				return player.IsQuest() && !player.IsInVR();
			}

			internal static Dictionary<string, NameplateText> Nameplates = new Dictionary<string, NameplateText>();
		public enum ClientDetectionLevel
			{
				Using,
				Likely_Using,
				Not_Using
			}
		}
	}
