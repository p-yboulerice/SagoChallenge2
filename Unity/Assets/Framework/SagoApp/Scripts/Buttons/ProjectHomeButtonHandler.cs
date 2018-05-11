namespace SagoApp {
	
	using UnityEngine;
	using SagoApp;
	using SagoApp.Project;
	using System.Collections;
	
	public class ProjectHomeButtonHandler : MonoBehaviour {

		#region Events

		public System.Action OnWillNavigateToProject;

		#endregion


		#region Serialized Fields

		[SerializeField]
		public bool CalledManually;

		#endregion


		#region Public Properties

		virtual public HomeButton HomeButton {
			get {
				m_HomeButton = m_HomeButton ?? GetComponentInChildren<HomeButton>();
				return m_HomeButton;
			}
		}

		#endregion


		#region Private Properties

		private float HomeButtonShowDelay {
			get { return 1.0f; }
		}

		#endregion


		#region Fields

		private HomeButton m_HomeButton;

		#endregion


		#region MonoBehaviour

		protected void Awake() {
			if (this.HomeButton != null) {
				this.HomeButton.StartHidden = true;
				this.HomeButton.AutoShow = false;
				this.HomeButton.HideImmediate();
			}
		}

		protected void Start() {
			if (!this.CalledManually) {
				InitHomeButton();
			}
		}

		#endregion


		#region Public Methods

		public void InitHomeButton() {
			StartCoroutine(InitHomeButtonAsync());
		}

		#endregion


		#region Private Methods

		private IEnumerator InitHomeButtonAsync() {
            if (ProjectNavigator.Instance) {
                while (ProjectNavigator.Instance.IsBusy) {
                    yield return null;
                }
	            if (this.HomeButton != null) {
	            	yield return new WaitForSeconds(this.HomeButtonShowDelay);
		            this.HomeButton.Action = OnHomeButton;
		            this.HomeButton.Show();
		        }
            }
        }

        private void OnHomeButton() {
			if (ProjectNavigator.Instance) {
				if (this.OnWillNavigateToProject != null) {
					OnWillNavigateToProject();
				}
				ProjectNavigator.Instance.NavigateToProject();
			}
        }

        #endregion
	}
}