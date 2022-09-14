using Core.Character;
using UnityEngine;

namespace Core.Hazards
{
    public class Hazard : MonoBehaviour
    {
        public int damage = 1;
        [SerializeField] bool destoryAfterTrigger=false;//是否在触发后进行自我销毁
        public event System.Action onHazardDestory;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            CheckCollision(collision.gameObject);
            if (destoryAfterTrigger)
            {
                onHazardDestory?.Invoke();
                Destroy(gameObject);//只能销毁自身，父物体依旧存在
            }
        }
        private void CheckCollision(GameObject collider)
        {
            if (collider.CompareTag("Player"))
            {
                var player = collider.GetComponent<PlayerCtrl>();
                if (!player.CanBeHit) return;

                var recoilDirection = (collider.transform.position - transform.position).normalized;
                //float multiplier = recoilDirection.y < 0 ? 1.0f : 500.0f;
                //Vector2 recoilForce = recoilDirection * multiplier;

                player.Hurt(damage,Vector2.zero /*recoilForce*/, killRecoil: false);
            }
        }
    }
}