using System;
using System.Collections;
using Managers;
using PlayerBehaviour;
using Settings;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Pong
{
    public class PongBall : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Image imgPongBall;
        [SerializeField] private float speed;
        [SerializeField] private Vector2 spawnPoint;
        [SerializeField] private float spawnDelay;
        [SerializeField] private float maxVerticalSpawnVelocity;

        private void Awake() => AddRandomVelocity();

        /// <summary>
        /// Handles collision.
        /// If a wall is hit (top or bottom), entrance angle == exit angle.
        /// If a player is hit (aka. the pong-bat), calculate new direction.
        /// If a player goal is hit, add point and respawn.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Consts.Tags.BORDER))
            {
                rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
                return;
            }

            if (other.TryGetComponent(out Player player))
            {
                float newSpeed = speed;
                if (GameManager.Instance.SpeedChange && MiBandManager.Instance.ConnectedBands > player.PlayerIndex)
                    newSpeed *= GameManager.Instance.GetSpeedChange(player.PlayerIndex);
                
                float delta = player.GetHitDelta(transform.position.y);
                float direction = Mathf.Sign(rb.velocity.x);
                Vector2 velocity = new Vector2((1 - Mathf.Abs(delta)) * -direction, delta).normalized * newSpeed;
                rb.velocity = velocity;
                return;
            }

            if (!other.TryGetComponent(out PongGoal pongGoal))
                return;

            pongGoal.Goal();
            StartCoroutine(Respawn());
        }

        /// <summary>
        /// Respawns the pong ball in the middle and gives it a random direction.
        /// </summary>
        private IEnumerator Respawn()
        {
            rb.velocity = Vector2.zero;
            imgPongBall.enabled = false;
            yield return new WaitForSeconds(spawnDelay);
            ((RectTransform) transform).anchoredPosition = spawnPoint;
            imgPongBall.enabled = true;
            AddRandomVelocity();
        }

        /// <summary>
        /// Adds a random velocity to the pong ball.
        /// </summary>
        private void AddRandomVelocity()
        {
            Vector2 direction = new Vector2(Random.Range(-1f, 1f),
                Random.Range(-maxVerticalSpawnVelocity, maxVerticalSpawnVelocity));
            rb.velocity = direction.normalized * speed;
        }
    }
}