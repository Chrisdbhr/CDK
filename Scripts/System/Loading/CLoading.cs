using System;
using UnityEngine;
using UniRx;

namespace CDK {
    [Obsolete]
	public class CLoading {

		private CLoadingCanvas _loadingCanvas;
		private readonly CRetainable _loadingUIRetainable;

		private CompositeDisposable _disposables;
		

		public CLoading() {
			this._disposables?.Dispose();
			this._disposables = new CompositeDisposable();
			
			this._loadingCanvas = CAssets.LoadAndInstantiate<CLoadingCanvas>("Loading Canvas");
			
			this._loadingUIRetainable = new CRetainable();

			this._loadingUIRetainable.IsRetainedRx.Subscribe(retained => {
				if (retained) this._loadingCanvas.ShowLoadingUI();
				else this._loadingCanvas.HideLoadingUI();
			})
			.AddTo(this._disposables);

			CApplication.QuittingEvent += () => {
				this._disposables?.Dispose();
			};
		}
		
		
		
		
		public void LoadingCanvasRetain() {
			_loadingUIRetainable.Retain();
		}

		public void LoadingCanvasRelease() {
			_loadingUIRetainable.Release();
		}
	}
}
