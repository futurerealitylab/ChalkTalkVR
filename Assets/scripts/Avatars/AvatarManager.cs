using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Text;

public class AvatarManager : Holojam.Network.Controller {
    [SerializeField]
    public string label = "Avatar1";

    public override void ResetData()
    {
        base.ResetData();
        Initialize();
    }

    protected override void Update()
    {
        if (this.Tracked) {
            // for local
            if (isLocal) {
                // we send the data in func OnLocalAvatarPacketRecorded
            } else {
                // we will receive the data and decode
                //print("recv ints[0]: " + data.ints[0]);
                //print("recv bytes: " + BitConverter.ToString(data.bytes));
                //print("recv data size: " + data.bytes.Length);
                DeserializeAndQueuePacketData(data.bytes);
            }
        }
    }

    public override bool Sending {
        get { return isLocal; }
    }

    public OvrAvatar ovrAvatar;
    public bool isLocal; // if it is local, packet the information and send, if it is remote, receive and decode the information
    private List<byte[]> packetData = new List<byte[]>();
    private List<byte> latestPosture = new List<byte>();

    public void disable()
    {
        ovrAvatar.RecordPackets = false;
        if (isLocal)
            ovrAvatar.PacketRecorded -= OnLocalAvatarPacketRecorded;
    }

    public void Initialize()
    {
        if (isLocal) {
            ovrAvatar.RecordPackets = true;
            ovrAvatar.PacketRecorded += OnLocalAvatarPacketRecorded;
        }
    }

    public List<byte> GetLatestPosture()
    {
        return latestPosture;
    }

    private int localSequence;

    public override string Label {
        get { return label; }
    }

    public void OnLocalAvatarPacketRecorded(object sender, OvrAvatar.PacketEventArgs args)
    {
        using (MemoryStream outputStream = new MemoryStream()) {
            BinaryWriter writer = new BinaryWriter(outputStream);

            var size = Oculus.Avatar.CAPI.ovrAvatarPacket_GetSize(args.Packet.ovrNativePacket);
            byte[] avatardata = new byte[(int)size];
            Oculus.Avatar.CAPI.ovrAvatarPacket_Write(args.Packet.ovrNativePacket, size, avatardata);
            latestPosture.Clear();
            latestPosture.AddRange(avatardata);
            //print("send seq: " + localSequence);
            //print("send size: " + size + "\t" + (int)size);
            //print("send avatardata: " + BitConverter.ToString(avatardata));
            // end of trick
            writer.Write(localSequence++);
            //writer.Write(ConnectionManager.LocalOculusID);
            writer.Write((int)size);
            writer.Write(avatardata);

            //packetData.Add(outputStream.ToArray());
            // here we only send current outputStream.ToArray()
            int sendSize = outputStream.ToArray().Length;
            //print("send data size: " + sendSize);
            data = new Holojam.Network.Flake(0, 0, 0, 1, sendSize+1, false);
            data.ints[0] = sendSize;
            //data.bytes = outputStream.ToArray();
            outputStream.ToArray().CopyTo(data.bytes, 1);
            // zhenyi debug
            //byte[] databytes = new byte[4] { 10, 20, 30, 16 };
            //data.bytes = new byte[5];
            //databytes.CopyTo(data.bytes, 1);
            //print("send bytes: " + BitConverter.ToString(data.bytes));
            //print(data.bytes.ToString());
            //print("stream: " + Encoding.Default.GetString(data.bytes));
            // test
            //DeserializeAndQueuePacketData(data.bytes);
        }
    }

    private void DeserializeAndQueuePacketData(byte[] avatardata)
    {
        using (MemoryStream inputStream = new MemoryStream(avatardata)) {
            if (avatardata.Length < 4)
            {
                Debug.LogWarning("avatardata.Length < 4");
                return;
            }
                
            BinaryReader reader = new BinaryReader(inputStream);
            int remoteSequence = reader.ReadInt32();
            //ulong remoteAvatarId = (ulong)reader.ReadUInt64();
            int size = reader.ReadInt32();
            byte[] sdkData = reader.ReadBytes(size);
            System.IntPtr packet = Oculus.Avatar.CAPI.ovrAvatarPacket_Read((System.UInt32)size, sdkData);
            print("recv seq: " + remoteSequence);
            print("recv size: " + size);
            print("recv avatardata: " + BitConverter.ToString(sdkData));
            // testing
            ovrAvatar.GetComponent<OvrAvatarRemoteDriver>().QueuePacket(remoteSequence, new OvrAvatarPacket { ovrNativePacket = packet });
            //this.GetComponent<SpacetimeAvatar>().DriveParallelOrGhostAvatarPosture(remoteSequence, sdkData);
        }
    }
}
