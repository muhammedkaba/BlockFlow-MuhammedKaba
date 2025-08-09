using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MKDev
{
    public class ParticleManager : Singleton<ParticleManager>
    {
        [SerializeField] private ParticleSystem[] pfParticles;

        public async UniTaskVoid PlayParticle(Vector3 pos, ParticleType type)
        {
            ParticleSystem ps = Instantiate(pfParticles[(int)type], pos, Quaternion.identity);

            await UniTask.Delay(5000);

            if (ps) Destroy(ps.gameObject);
        }
    }

    public enum ParticleType
    {
        BlockUnlocked
    }
}
