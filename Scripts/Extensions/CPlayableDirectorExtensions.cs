using UnityEngine.Playables;

namespace CDK {
	public static class CPlayableDirectorExtensions {

		public static void CSetAsActiveAndPlay(this PlayableDirector p) {
			if (p == null) return;
			p.gameObject.SetActive(true);
			p.Play();
		}
		
	}
}
