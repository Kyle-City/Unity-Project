using UnityEngine;

public class PickupBulletBoost : MonoBehaviour
{
    [Header("Buff 参数")]
    public float attackCooldownMultiplier = 0.6f;
    public float bulletSpeedMultiplier = 1.5f;
    public float duration = 8f;

    [Header("拾取音效（2D）")]
    public AudioClip pickupSfx;
    [Range(0f, 1f)]
    public float pickupVolume = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.ApplyBulletBoost(attackCooldownMultiplier, bulletSpeedMultiplier, duration);

            PlayPickupSfx2D();

            Destroy(gameObject);
        }
    }

    private void PlayPickupSfx2D()
    {
        if (pickupSfx == null) return;

        // 创建一个临时物体来播放音效（2D，不受距离影响）
        GameObject go = new GameObject("PickupSFX_2D");
        AudioSource src = go.AddComponent<AudioSource>();

        src.clip = pickupSfx;
        src.volume = pickupVolume * 2f;
        src.spatialBlend = 0f; // 0 = 2D（关键！）
        src.playOnAwake = false;

        src.Play();

        Destroy(go, pickupSfx.length + 0.1f); // 播完再销毁
    }
}
