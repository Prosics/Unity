using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prosics.Util;
namespace Prosics
{
	public class AudioManager : MonoScriptBase 
	{
		const string audioPath = "default/sound/";
		static float _volume = 1f;
		public static float volume
		{
			get{return _volume;}
			set
			{ 
				if ( _volume != value )
				{
					_volume = value;
					if (_instance != null)
					{
						if(_instance._serialAudioPlayer != null)
							_instance._serialAudioPlayer.audioSource.volume =_instance._serialAudioPlayer.clipsVolume[_instance._serialAudioPlayer.curIdx] * volume;

						foreach (KeyValuePair<int,AudioPlayerInfo> kv in _instance._parallelAudioPlayers)
						{
							kv.Value.audioSource.volume = kv.Value.clipsVolume[kv.Value.curIdx] * volume;
						}
					}
				}

			}
		}
		static AudioManager _instance = null;
		public static AudioManager instance
		{
			get 
			{ 
				if(_instance == null)
				{
					GameObject go = new GameObject ();
					go.AddComponent<AudioManager> ();
				}
				return _instance;
			}
		}
		internal class AudioPlayerInfo
		{
			public int id = 0;
			public List<string> audioList = new List<string>();
			public PlayMode playMode = PlayMode.Order;
			public AudioSource audioSource = null;
			//该音效的
			public List<float> clipsVolume  =new List<float>();
			//当前播放的索引。
			public int curIdx = 0;

			public AudioPlayerInfo()
			{
				
			}
			public AudioPlayerInfo(int id,AudioSource audioSource ,PlayMode playMode,List<string> audioList,List<float> clipsVolume)
			{
				this.id = id;
				this.audioSource = audioSource;
				this.playMode = playMode;
				this.audioList = audioList;
				this.clipsVolume = clipsVolume ?? new List<float>();
				for(int i =0; i<this.audioList.Count; i++)
				{
					if(i >= this.clipsVolume.Count)
						this.clipsVolume.Add(1f);
				}
			}
			public void Reset(PlayMode playMode,List<string> audioList,List<float> clipsVolume)
			{
				this.playMode = playMode;
				this.audioList = audioList;
				this.clipsVolume = clipsVolume ?? new List<float>();
				for(int i =0; i<this.audioList.Count; i++)
				{
					if(i >= this.clipsVolume.Count)
						this.clipsVolume.Add(1f);
				}
			}


			public void Stop()
			{
				audioList = new List<string>();
				audioSource.clip = null;
				curIdx = 0;
			}

		};

		public enum PlayMode
		{
			Order,
			Loop,
			Random
		}



		//音频的对象池。
		IDictionary<string,AudioClip> _clipDic = new Dictionary<string, AudioClip>();
		AudioPlayerInfo _serialAudioPlayer = null;

		IDictionary<int,AudioPlayerInfo> _parallelAudioPlayers= new Dictionary<int, AudioPlayerInfo>();


		//未来需要实现的功能





		protected override void Awake()
		{
			base.Awake ();
			_instance = this;
			gameObject.name = this.GetType().ToString();
			GameObject.DontDestroyOnLoad (gameObject);


		}
		protected override void Start()
		{
			base.Start ();
			StartCoroutine (AudioPlayerMonitor());
		}
		protected override void Update()
		{
			base.Update ();
		}

		AudioPlayerInfo CreateAudioPlayer()
		{
			AudioPlayerInfo progress = new AudioPlayerInfo();
			AudioSource source = gameObject.AddComponent<AudioSource> ();
			source.volume = volume;
			progress.audioSource = source;
			return progress;
		}
		#region 播放控制
		IEnumerator AudioPlayerMonitor()
		{
			while(true)
			{
				//处理播放列表顺序播放
				AudioPlayerIterate ();
				//回收已播放完毕的播放器  仅用于并行播放
				//AudioPlayerClean();

				yield return new WaitForFixedUpdate ();
			}
		}

		void AudioPlayerIterate()
		{
			List<AudioPlayerInfo> list = new List<AudioPlayerInfo> ();
			if(_serialAudioPlayer != null)
				list.Add (_serialAudioPlayer);
			list.AddRange (_parallelAudioPlayers.Values);
			AudioPlayerInfo player = null;
			AudioClip clip = null;
			for (int i = 0; i < list.Count; i++)
			{
				player = list [i];
				if ( !player.audioSource.isPlaying )
				{
					//如果播放已停止，检测 播放列表.
					if ( player.playMode == PlayMode.Order )
					{
						player.curIdx = player.curIdx + 1;
						if ( player.curIdx < player.audioList.Count )
						{
							Play (player);
						}
						else
							RemoveAudioPlayer (player.id);
					}
					else if ( player.playMode == PlayMode.Loop )
					{
						player.curIdx = (player.curIdx + 1) %player.audioList.Count;
						Play (player);
					}
					else if ( player.playMode == PlayMode.Random )
					{
						player.curIdx = Random.Range (0, player.audioList.Count);
						Play (player);
					}
				}
			}

		}
		void RemoveAudioPlayer(int id)
		{
			//不删除串行播放器
			if ( _parallelAudioPlayers.ContainsKey (id) )
			{
				AudioPlayerInfo player = _parallelAudioPlayers [id];
				_parallelAudioPlayers.Remove (id);
				Destroy (player.audioSource);
			}
		}
		#endregion








		//释放已加载的音频，可在场景切换时调用。
		public void Release()
		{

		}
		public void StopSerialPlayer()
		{
			StopPlayer (0);
		}
		public void StopPlayer(int id)
		{
			if ( id == 0 &&  _serialAudioPlayer!= null)///error
				_serialAudioPlayer.Stop ();
			else
				RemoveAudioPlayer (id);
		}
		public void StopAllPlayer()
		{
			StopSerialPlayer ();
			List<int> ids = new List<int> (_parallelAudioPlayers.Keys);
			foreach (int id in ids)
			{
				RemoveAudioPlayer (id);
			}
		}

		#region 串行播放
		/// <summary>
		/// 播放一个音频，会停止上一个串行播放的音频进程
		/// </summary>
		/// <param name="audioName">Audio name.</param>
		/// <param name="loop">是否循环播放</param>
		public void Play_Serial(string audioName,bool loop = false)
		{
			List<string> list = new List<string> ();
			list.Add (audioName);
			if(loop)
				Play_Serial (list,PlayMode.Loop);
			else
				Play_Serial (list,PlayMode.Order);
		}

		public void Play_Serial(string audioName,float volume ,bool loop = false)
		{
			List<string> list = new List<string> ();
			list.Add (audioName);
			List<float> volumes = new List<float> ();
			volumes.Add (volume);
			if(loop)
				Play_Serial (list,volumes,PlayMode.Loop);
			else
				Play_Serial (list,volumes,PlayMode.Order);
		}
		/// <summary>
		/// 播放一个音频，会停止上一个串行播放的音频进程
		/// </summary>
		/// <param name="audioNames">音频列表</param>
		/// <param name="playMode">播放模式</param>
		public void Play_Serial(List<string> audioNames, PlayMode playMode = PlayMode.Order)
		{
			if(audioNames == null || audioNames.Count == 0)
				return;
			Play_Serial (audioNames, null, playMode);
		}
		public void Play_Serial(List<string> audioNames,List<float> volumes, PlayMode playMode = PlayMode.Order)
		{
			if(audioNames == null || audioNames.Count == 0)
				return;
			if (_serialAudioPlayer == null)
				_serialAudioPlayer = new AudioPlayerInfo (
					0,
					gameObject.AddComponent<AudioSource> (),
					playMode,
					audioNames,
					volumes);
			else
				_serialAudioPlayer.Reset (playMode,audioNames,volumes);
			Play (_serialAudioPlayer);
		}
		#endregion





		#region 并行播放
		/// <summary>
		/// 并行播放一个音频，每次调用都会产生一个新的音频播放进程，多个进程可同时播放。
		/// </summary>
		/// <returns>返回一本次播放的ID，通过此ID可停止此进程</returns>
		/// <param name="audioName">Audio name.</param>
		/// <param name="loop"> 是否循环播放</param>
		/// <param name="replaceSame">如果已存在一个与此音频同名的播放进程是否替换</param>
		public int Play_Parallel(string audioName,bool loop = false)
		{
			List<string> list = new List<string> ();
			list.Add (audioName);
			if(loop)
				return Play_Parallel (list,PlayMode.Loop);
			else
				return Play_Parallel (list,PlayMode.Order);
		}
		public int Play_Parallel(string audioName,float volume,bool loop = false)
		{
			List<string> list = new List<string> ();
			list.Add (audioName);
			List<float> volumes = new List<float> ();
			volumes.Add (volume);
			if(loop)
				return Play_Parallel (list,volumes,PlayMode.Loop);
			else
				return Play_Parallel (list,volumes,PlayMode.Order);
		}


		/// <summary>
		/// 并行播放多个音频，每次调用都会产生一个新的音频播放进程，多个进程可同时播放。
		/// </summary>
		/// <returns>返回一本次播放的ID，通过此ID可停止此进程</returns>
		/// <param name="audioNames">Audio names.</param>
		/// <param name="playMode">播放模式</param>
		public int Play_Parallel(List<string> audioNames,PlayMode playMode = PlayMode.Order)
		{
			return Play_Parallel(audioNames,null,playMode);
		}
		public int Play_Parallel(List<string> audioNames,List<float> volumes,PlayMode playMode = PlayMode.Order)
		{
			if(audioNames == null || audioNames.Count ==0)
				return -1;
			AudioPlayerInfo player = new AudioPlayerInfo (
				                         GeneratePlayerId (),
				                         gameObject.AddComponent<AudioSource> (),
				                         playMode,
				                         audioNames,
				                         volumes);
			_parallelAudioPlayers.Add(player.id,player);
			Play(player);
			return player.id;
		}

		#endregion

		void Play(AudioPlayerInfo player)
		{
			if ( player.curIdx < player.audioList.Count )
			{
				AudioClip clip =	FindAudioClip (player.audioList[player.curIdx]);
				player.audioSource.clip = clip;
				player.audioSource.volume =player.clipsVolume[player.curIdx]* volume;
				player.audioSource.Play ();
			}
		}

		AudioClip FindAudioClip(string clipName)
		{
			if ( _clipDic.ContainsKey (clipName) )
				return _clipDic [clipName];
			else
			{
				Object obj = Resources.Load (audioPath + clipName);
				if ( obj != null )
				{
					_clipDic.Add (clipName, obj as AudioClip);
					return obj as AudioClip;
				}
			}
			return null;
		}

		int _nextId = 0;
		int GeneratePlayerId()
		{
			_nextId += 1;
			return _nextId; 
		//	return Random.Range (1,int.MaxValue);
		}
			











		void StopSynAudio(string audioName)
		{
		}
		void StopAsynAudio(string audioName)
		{
		}











	}
}
