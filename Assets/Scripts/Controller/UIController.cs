using Model;
using System;
using View;

namespace Controller {
    public class UIController {

        private MainStateModel _mainStateModel;
        private UIView _uIView;

        internal void Init() {
            
            //assign local references
            _mainStateModel = ModelInitiator.GetMainStateModel();
            _uIView = ViewInitiator.GetUIView();

            //init view
            _uIView.Init();
        }

        internal void UpdateState() {
            _uIView.UpdateState();
        }

    }
}

