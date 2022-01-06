using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unity.Netcode
{
    /// <summary>
    /// A helper struct for serializing <see cref="NetworkObject"/>s over the network. Can be used in RPCs and <see cref="NetworkVariable{T}"/>.
    /// </summary>
    public struct NetworkObjectReference : INetworkSerializable, IEquatable<NetworkObjectReference>
    {
        private ulong m_NetworkObjectId;

        /// <summary>
        /// The <see cref="NetworkObject.NetworkObjectId"/> of the referenced <see cref="NetworkObject"/>.
        /// </summary>
        public ulong NetworkObjectId
        {
            get => m_NetworkObjectId;
            internal set => m_NetworkObjectId = value;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="NetworkObjectReference"/> struct.
        /// </summary>
        /// <param name="networkObject">The <see cref="NetworkObject"/> to reference.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public NetworkObjectReference(NetworkObject networkObject)
        {
            if (networkObject == null)
            {
                throw new ArgumentNullException(nameof(networkObject));
            }

            if (networkObject.IsSpawned == false)
            {
                throw new ArgumentException($"{nameof(NetworkObjectReference)} can only be created from spawned {nameof(NetworkObject)}s.");
            }

            m_NetworkObjectId = networkObject.NetworkObjectId;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="NetworkObjectReference"/> struct.
        /// </summary>
        /// <param name="gameObject">The GameObject from which the <see cref="NetworkObject"/> component will be referenced.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public NetworkObjectReference(GameObject gameObject)
        {
            if (gameObject == null)
            {
                throw new ArgumentNullException(nameof(gameObject));
            }

            var networkObject = gameObject.GetComponent<NetworkObject>();

            if (networkObject == null)
            {
                throw new ArgumentException($"Cannot create {nameof(NetworkObjectReference)} from {nameof(GameObject)} without a {nameof(NetworkObject)} component.");
            }

            if (networkObject.IsSpawned == false)
            {
                throw new ArgumentException($"{nameof(NetworkObjectReference)} can only be created from spawned {nameof(NetworkObject)}s.");
            }

            m_NetworkObjectId = networkObject.NetworkObjectId;
        }

        /// <summary>
        /// Tries to get the <see cref="NetworkObject"/> referenced by this reference.
        /// </summary>
        /// <param name="networkObject">The <see cref="NetworkObject"/> which was found. Null if no object was found.</param>
        /// <param name="networkManager">The networkmanager. Uses <see cref="NetworkManager.Singleton"/> to resolve if null.</param>
        /// <returns>True if the <see cref="NetworkObject"/> was found; False if the <see cref="NetworkObject"/> was not found. This can happen if the <see cref="NetworkObject"/> has not been spawned yet. you can try getting the reference at a later point in time.</returns>
        public bool TryGet(out NetworkObject networkObject, NetworkManager networkManager = null)
        {
            networkObject = Resolve(this, networkManager);
            return networkObject != null;
        }

        /// <summary>
        /// Resolves the corresponding <see cref="NetworkObject"/> for this reference.
        /// </summary>
        /// <param name="networkObjectRef">The reference.</param>
        /// <param name="networkManager">The networkmanager. Uses <see cref="NetworkManager.Singleton"/> to resolve if null.</param>
        /// <returns>The resolves <see cref="NetworkObject"/>. Returns null if the networkobject was not found</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NetworkObject Resolve(NetworkObjectReference networkObjectRef, NetworkManager networkManager = null)
        {
            networkManager = networkManager != null ? networkManager : NetworkManager.Singleton;
            networkManager.SpawnManager.SpawnedObjects.TryGetValue(networkObjectRef.m_NetworkObjectId, out NetworkObject networkObject);

            return networkObject;
        }

        /// <inheritdoc/>
        public bool Equals(NetworkObjectReference other)
        {
            return m_NetworkObjectId == other.m_NetworkObjectId;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is NetworkObjectReference other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return m_NetworkObjectId.GetHashCode();
        }

        /// <inheritdoc/>
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref m_NetworkObjectId);
        }

        public static implicit operator NetworkObject(NetworkObjectReference networkObjectRef) => Resolve(networkObjectRef);

        public static implicit operator NetworkObjectReference(NetworkObject networkObject) => new NetworkObjectReference(networkObject);

        public static implicit operator GameObject(NetworkObjectReference networkObjectRef) => Resolve(networkObjectRef).gameObject;

        public static implicit operator NetworkObjectReference(GameObject gameObject) => new NetworkObjectReference(gameObject);
    }
}
