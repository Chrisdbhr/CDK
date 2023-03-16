using UnityEngine;

namespace CDK {
    /// <summary>
    /// Find and follow a Player by Tag.
    /// </summary>
    public class CFindAndFollowAPlayer : CTransformFollower {
        [SerializeField] [CTagSelector] protected string _playerTag = "Player";

        protected override void Execute(float deltaTime) {
            if (this.TransformToFollow == null) {
                GameObject.FindWithTag(this._playerTag)
                .CDoIfNotNull(g => this.SetTransformToFollow(g.transform));
            }
            base.Execute(deltaTime);
        }
    }
}