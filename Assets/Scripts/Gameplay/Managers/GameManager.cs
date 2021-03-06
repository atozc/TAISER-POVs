using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : Core.Utilities.SingletonPun<GameManager> {
	// Events called when a wave starts or ends
    public static Utilities.VoidEventCallback waveStartEvent;
	public static Utilities.VoidEventCallback waveEndEvent;
	// Event called when the game ends
	public static Utilities.VoidEventCallback gameEndEvent;


	// Enum providing possible difficulty values
	[System.Serializable]
	public enum Difficulty {
		Easy = 0,
		Medium = 1,
		Hard = 2
	}
	// Static property describing the difficulty of the game
	static Difficulty _difficulty = Difficulty.Easy;
	public static Difficulty difficulty{
		get => _difficulty;
		set {
			// If an instance exists then network sync any changes
			if(instanceExists) instance.SetDifficulty(difficulty);
			// If an instance doesn't exist, then just hold the new value
			else _difficulty = difficulty;
		}
	}

	// Setting which determines the maximum number of waves before the game ends
	public int maximumWaves = 3;
	// Property determining the current wave
	[SerializeField] int _currentWave = 0;
	public int currentWave {
		get => _currentWave;
		protected set => _currentWave = value;
	}

	// Property which returns true if the wave is currently started
	bool _waveStarted = false;
	public bool waveStarted {
		get => _waveStarted;
		protected set => _waveStarted = value;
	}

    // Prefabs to spawn containing the managers specific to each side
    public GameObject whiteHatPlayerPrefab;
    public GameObject whiteHatAdvisorPrefab, blackHatPlayerPrefab, blackHatAdvisorPrefab, observerPrefab;


	// When the scene starts spawn the correct side
	new void Awake(){
		base.Awake();

		if(NetworkingManager.isObserver){
			 Debug.Log("Instantiating prefabs for Observer...");
			 Instantiate(observerPrefab).name = "Observer Managers";
		} else if(NetworkingManager.isBlackHat && NetworkingManager.isPrimary) Instantiate(blackHatPlayerPrefab).name = "BlackHat Player Managers";
		else if(NetworkingManager.isBlackHat) Instantiate(blackHatAdvisorPrefab).name = "BlackHat Advisor Managers";
		else if(NetworkingManager.isWhiteHat && NetworkingManager.isPrimary) Instantiate(whiteHatPlayerPrefab).name = "WhiteHat Player Managers";
		else Instantiate(whiteHatAdvisorPrefab).name = "WhiteHat Advisor Managers";
	}

	// When we are dis/enabled register ourselves as a listener to playerPropertyUpdateEvents and roomOtherLeaveEvent
	void OnEnable(){
		NetworkingManager.roomPlayerPropertiesUpdateEvent += OnPlayerRoomPropertiesUpdate;
		NetworkingManager.roomOtherLeaveEvent += OnOtherPlayerLeave;
	}
	void OnDisable(){
		NetworkingManager.roomPlayerPropertiesUpdateEvent -= OnPlayerRoomPropertiesUpdate;
		NetworkingManager.roomOtherLeaveEvent -= OnOtherPlayerLeave;
	}

	void Start(){
		// Make sure that the graph is rebuilt before we start
		PathNodeBase.UpdateGraphConnections();

		// When we load into the scene make sure that every player is not readied
		if(NetworkingManager.instance) NetworkingManager.instance.setReady(false);

		// When the game starts ensure that the difficulty level is synced with all of the players
		SetDifficulty(difficulty);

		if(NetworkingManager.instance) // Skip this step in debugging
		// Transfer control of the starting points and destinations to the BlackHatPlayer
		if(NetworkingManager.isHost){
			foreach(StartingPoint p in StartingPoint.startingPoints)
				if(NetworkingManager.blackHatPrimaryPlayer is object)
					p.photonView.TransferOwnership(NetworkingManager.blackHatPrimaryPlayer);
			foreach(Destination d in Destination.destinations)
				if(NetworkingManager.blackHatPrimaryPlayer is object)
					d.photonView.TransferOwnership(NetworkingManager.blackHatPrimaryPlayer);
		}
	}

	void Update(){
		// If the wave has been started, check to see if there are any packets still remaining
		if(waveStarted && !PacketEntityPoolManager.instance.packetsExist)
			EndWave();
	}

	// Helper function used to set the ready state even when debugging without a networking manager instance
	void setReady(bool isReady){
		// Play a sound to indicate that we are ready
		if(isReady) AudioManager.instance.uiSoundFXPlayer.PlayTrackImmediate("SettingsUpdated");

		if(NetworkingManager.instance)
			NetworkingManager.instance.setReady(isReady);
		else if(isReady) StartNextWave();
	}


	// -- Callbacks --


	// When player properties are updated, adjust the ready state and possibly start the game
	public void OnPlayerRoomPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable propertiesThatChanged){
		// Update the ready button text for this player
		BaseUI.instance.readyText.text = (NetworkingManager.isReady ? "Unready" : "Ready");

		// If all of the players are ready, then start the next wave (Network Synced, Function ignores calls from everyone but host)
		if(NetworkingManager.instance.allPlayersReady())
			StartNextWave();
	}

	// When the ready up button is pressed... ready up
	public void toggleReady(){
		setReady(!NetworkingManager.isReady);
	}

	// Whenever the disconnect button (or return to main menu button in the win screen) is pressed, leave the game and go back to the main menu
	public void OnDisconnectButtonPressed(){
		NetworkingManager.instance.LeaveRoomAndReturnToMainMenu();
		gameEndEvent?.Invoke();
	}

	// When the other play leaves the game, the player who remains wins the game
	public void OnOtherPlayerLeave(Player otherPlayer){
		// If the white hat side no longer has its primary player then the blackhats win
		if(NetworkingManager.whiteHatPrimaryPlayer is null)
			EndGame(Networking.Player.Side.BlackHat);
		// If the black hat side no longer has its primary player then the whitehats win
		else if(NetworkingManager.blackHatPrimaryPlayer is null)
			EndGame(Networking.Player.Side.WhiteHat);
	}


	// -- Network Sync --


	// Function which starts the next wave (Network Synced, ignores calls from players who aren't the host)
	public void StartNextWave() {
        if(NetworkingManager.isHost)
            photonView.RPC("RPC_GameManager_StartNextWave", RpcTarget.AllBuffered);
    }
	[PunRPC] void RPC_GameManager_StartNextWave(){
		Debug.Log("Starting Next Wave!");

		// Ensure that once the wave starts, all of the players are marked as unready
		if(NetworkingManager.isHost) NetworkingManager.instance.UnreadyAllPlayers();

		waveStartEvent?.Invoke();
		waveStarted = true; // Mark that the wave has started
	}

	// Function which ends the current wave (Network Synced, ignores calls from players who aren't the host)
	public void EndWave() { if(NetworkingManager.isHost) photonView.RPC("RPC_GameManager_EndWave", RpcTarget.AllBuffered); }
	[PunRPC] void RPC_GameManager_EndWave(){
		Debug.Log("Wave Ended!");

		waveEndEvent?.Invoke();
		waveStarted = false; // mark that the wave has ended

		// Increase the current wave
		currentWave++;
		// If the current wave is greater than the maximum number of waves... end the game
		if(currentWave >= maximumWaves)
			EndGame(ScoreManager.instance.whiteHatScore >= ScoreManager.instance.blackHatScore ? Networking.Player.Side.WhiteHat : Networking.Player.Side.BlackHat);
	}

	// Function which ends the game, marking which player won (Network Synced)
	public void EndGame(Networking.Player.Side winningSide) { if(NetworkingManager.isHost) photonView.RPC("RPC_GameManager_EndGame", RpcTarget.AllBuffered, winningSide); }
	[PunRPC] void RPC_GameManager_EndGame(Networking.Player.Side winningSide){
		try{
			// TODO: Need to take Observers into account (right now they always lose)
			// Show the win text if the player's side won
			if (NetworkingManager.localPlayer.role == Networking.Player.Role.Observer) // Is Observer
				BaseUI.instance.gameEndText.SetActive(true); 
			else if (winningSide == NetworkingManager.localPlayer.side)
				BaseUI.instance.winText.SetActive(true);
			else BaseUI.instance.loseText.SetActive(true);
		// If we fail to find the index then assume that we lost
		} catch (System.IndexOutOfRangeException) { BaseUI.instance.loseText.SetActive(true); }

		// Show the game over screen
		BaseUI.instance.endGameScreen.SetActive(true);
		gameEndEvent?.Invoke();
	}

	// Function which sets the difficulty (Network Synced, ignores calls from players who aren't the host)
	public void SetDifficulty(Difficulty diff) { if(NetworkingManager.isHost) photonView.RPC("RPC_GameManager_SetDifficulty", RpcTarget.AllBuffered, diff); }
	[PunRPC] void RPC_GameManager_SetDifficulty(Difficulty diff) {
		_difficulty = diff;
	}


	// -- Hat Specific RPC Forwarding --


	// -- WhiteHat --

	[PunRPC] void RPC_WhiteHatBaseManager_ProposeNewFirewallFilterRules(int firewallID, string rules){
		if(WhiteHatBaseManager.instance is null) return;
		WhiteHatBaseManager.instance.OnProposedNewFirewallFilterRules(Firewall.GetFromID((uint) firewallID), PacketRule.Parse(rules));
	}

	[PunRPC] void RPC_WhiteHatBaseManager_ProposeMakeDestinationHoneypot(int destinationID){
		if(WhiteHatBaseManager.instance is null) return;
		WhiteHatBaseManager.instance.OnProposedMakeDestinationHoneypot(Destination.GetFromID((uint) destinationID));
	}

	// -- BlackHat --

	[PunRPC] void RPC_BlackHatBaseManager_ProposeNewStartPointMalciousPacketRules(int startPointID, string rules){
		if(BlackHatBaseManager.instance is null) return;
		BlackHatBaseManager.instance.OnProposedNewStartPointMalciousPacketRules(StartingPoint.GetFromID((uint) startPointID), PacketRule.Parse(rules));
	}

	[PunRPC] void RPC_BlackHatBaseManager_ProposeNewStartPointMaliciousPacketProbability(int startPointID, float probability){
		if(BlackHatBaseManager.instance is null) return;
		BlackHatBaseManager.instance.OnProposedNewStartPointMaliciousPacketProbability(StartingPoint.GetFromID((uint) startPointID), probability);
	}

	[PunRPC] void RPC_BlackHatBaseManager_ProposeNewDestinationMaliciousPacketTargetLikelihood(int destinationID, int likelihood){
		if(BlackHatBaseManager.instance is null) return;
		BlackHatBaseManager.instance.OnProposedNewDestinationMaliciousPacketTargetLikelihood(Destination.GetFromID((uint) destinationID), likelihood);
	}
}
