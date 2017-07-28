﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MineCase.Protocol;
using Orleans.Concurrency;
using Orleans;
using MineCase.Protocol.Handshaking;
using System.IO;

namespace MineCase.Server.Network
{
    partial class PacketRouterGrain : Grain, IPacketRouter
    {
        private SessionState _state;

        public async Task SendPacket(UncompressedPacket packet)
        {
            dynamic innerPacket = new object();
            switch (_state)
            {
                case SessionState.Handshaking:
                    innerPacket = DeserializeHandshakingPacket(ref packet);
                    break;
                case SessionState.Status:
                    innerPacket = DeserializeStatusPacket(ref packet);
                    break;
                case SessionState.Login:
                    innerPacket = DeserializeLoginPacket(ref packet);
                    break;
                case SessionState.Play:
                    break;
                case SessionState.Closed:
                    break;
                default:
                    break;
            }
            await DispatchPacket(innerPacket);
        }

        public async Task Close()
        {
            _state = SessionState.Closed;
            await GrainFactory.GetGrain<IClientboundPaketSink>(this.GetPrimaryKey()).Close();
            DeactivateOnIdle();
        }

        private Task DispatchPacket(object packet)
        {
            throw new NotImplementedException();
        }

        public Task Play()
        {
            _state = SessionState.Play;
            return Task.CompletedTask;
        }

        public enum SessionState
        {
            Handshaking,
            Status,
            Login,
            Play,
            Closed
        }
    }
}