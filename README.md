# ChalkTalkVR
Combine ChalkTalk with VR/AR experience

# AR configuration
1. Go to mirageAR branch
2. Change preview mode's actor id
3. Change VR-CT(1,2) for broadcasting
4. change imu(1,2) for broadcasting

## Unity project
1. Go to `scenes/chalktalk.unity`
2. Choose `Preview Mode` under `BuildManager` to specify build index
3. Modify `Relay Address` as you need.

## Relay
1. Copy `relay/index.js` to node folders under holojam_node
2. Run `node relay/frlrelay.js` with that holojam_node
3. Run `vivedeviceemitter.py` with default parameter under FRL.relay repo

## ChalkTalk
1. Run `server/main.js` to start the server
2. Open browser and go to localhost:11235 to run the client
