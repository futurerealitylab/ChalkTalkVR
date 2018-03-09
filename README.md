# ChalkTalkVR
Combine ChalkTalk with VR/AR experience

## Unity project
1. Go to `scenes/chalktalk_ar.unity`
2. Choose `Preview Mode` under `BuildManager` to specify build index
3. Modify `Relay Address` as you need.
4. Change `Label` of `TrackersForS8` to `VR-CT(1,2)`
5. Change `Label` of `imu` to `imu` or `imu2`

## Relay
1. Checkout branch `chalktalkAR` in repo FRL.Relay.
2. Run `node relay.js --dwPorts=[9591,9592]` for specific downstream ports.

## ChalkTalk
1. run `node server\main.js`
2. open browser with `localhost:11235`

## ViveEmitter
1. run `python ViveDeviceEmitter\vivedeviceemitter.py`
