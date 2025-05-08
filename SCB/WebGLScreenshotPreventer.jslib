/************************************************************
THIS ONLY BLOCKS THE FOLLOWING:
- Common screenshot shortcuts
- Inspect Element shortcuts
Screenshoting in Browser can be easily avoided and can't be relied on.
************************************************************/
var ScreenShotPreventer = {
    PROTECTIVE_CONTENT: false,
    IntervalCheckID: -1,
    DIVC: null,
    DIVLogo: null,
    CallNonEvents: null,
    IsBlurredBackground: false,

    ApplyProtectionWithAppName__deps: ["PROTECTIVE_CONTENT", "IntervalCheckID", "DIVC", "DIVLogo", "CallNonEvents", "IsBlurredBackground"],
    ApplyProtectionWithAppName: function (str, blur) {
        _IsBlurredBackground = blur;
        let gamename = UTF8ToString(str)
        const CreateModal = (ElementToInsert) => {
            const DIV = document.createElement('div');
            DIV.style.display = "none";
            DIV.style.top = "0";
            DIV.style.right = "0";
            DIV.style.position = "absolute";
            DIV.style.width = "100%";
            DIV.style.height = "100%";
            DIV.style.userSelect = "none";
            DIV.style.textAlign = "center";
            if (blur) DIV.style.backdropFilter = "blur(50px)";
            else {
                let ColorBG = "#000000";
                try {
                    ColorBG = window.getComputedStyle(document.querySelector("#unity-canvas"))['backgroundColor'];
                } catch (error) {

                }
                DIV.style.backgroundColor = ColorBG;
            }
            DIV.addEventListener('contextmenu', (ev1) => { ev1.preventDefault(); ev1.stopImmediatePropagation(); });
            ElementToInsert.appendChild(DIV);
            return DIV;
        };
        const CreateLogo = () => {
            const DIV = document.createElement('div');
            DIV.style.opacity = "100%";
            DIV.style.margin = "auto";
            DIV.style.width = "100%";
            DIV.style.top = "calc(50% - 30px)";
            DIV.style.position = "absolute";

            const LogoImg = document.createElement('img');
            LogoImg.src = "./TemplateData/unity-logo-dark.png";
            LogoImg.width = "64";
            LogoImg.height = "64";
            LogoImg.style.height = "90px";
            LogoImg.style.width = "90px";

            const Text = document.createElement('a');
            Text.style.fontFamily = "sans-serif";
            Text.innerText = gamename;
            Text.style.fontSize = "48px";
            let TextColor = "white";
            try {
                TextColor = window.getComputedStyle(document.querySelector("#unity-progress-bar-full"))['background'].includes('-dark') ? "white" : "black";
            } catch (error) {

            }
            Text.style.color = TextColor;
            Text.style.position = "relative";
            Text.style.bottom = "26px";
            DIV.appendChild(LogoImg);
            DIV.appendChild(Text);

            _DIVC.appendChild(DIV);
            return DIV;
        };
        if (_PROTECTIVE_CONTENT) return;
        _PROTECTIVE_CONTENT = true;
        let OnWhere = document.body;
        if (document.querySelector('#unity-canvas').style.width != "100%") {
            OnWhere = document.querySelector("#unity-container");
        }
        _DIVC = CreateModal(OnWhere);
        _DIVLogo = CreateLogo();
        let IsPageHidden = document.hidden || !document.hasFocus();
        let HasToClick = false;
        let CantClick = false;
        const Hide = () => {
            _DIVC.style.display = "block";
            if (_IsBlurredBackground) {
                _DIVC.style.backdropFilter = "blur(50px)";
            }
            else {
                _DIVC.style.opacity = "100%";
            }
            _DIVLogo.style.opacity = "0%";
            let AnimateBlur = 0;
            const a = setInterval(() => {
                if (!IsPageHidden || !_PROTECTIVE_CONTENT) {
                    clearInterval(a);
                    return;
                }
                AnimateBlur += 0.05;
                _DIVLogo.style.opacity = Math.min(100, Math.max(0, (AnimateBlur / 0.4) * 2.5 * 100)) + "%";
                if (AnimateBlur >= 0.4) {
                    clearInterval(a);
                    _DIVLogo.style.opacity = "100%";
                }
            }, 50);
        };
        const Show = () => {
            HasToClick = false;
            let AnimateUnblur = 0;
            _DIVLogo.style.opacity = "100%";
            if (!_IsBlurredBackground) {
                _DIVC.style.opacity = "100%";
            }
            const a = setInterval(() => {
                if (IsPageHidden || !_PROTECTIVE_CONTENT) {
                    clearInterval(a);
                    return;
                }
                AnimateUnblur += 0.05;
                if (_IsBlurredBackground) {
                    _DIVC.style.backdropFilter = "blur(" + Math.max(0, (0.4 - (AnimateUnblur / 0.4)) * 2.5 * 50) + "px)";
                    _DIVLogo.style.opacity = Math.min(100, Math.max(0, (0.4 - (AnimateUnblur / 0.4)) * 2.5 * 100)) + "%";
                }
                else {
                    _DIVC.style.opacity = Math.min(100, Math.max(0, (0.4 - (AnimateUnblur / 0.4)) * 2.5 * 100)) + "%";
                }
                if (AnimateUnblur >= 0.4) {
                    clearInterval(a);
                    _DIVC.style.display = "none";
                    if (_IsBlurredBackground) {
                        _DIVC.style.backdropFilter = "blur(50px)";
                        _DIVLogo.style.opacity = "100%";
                    }
                    else {
                        _DIVC.style.opacity = "100%";
                    }
                }
            }, 50);
        };
        const BlurEvent_1 = () => {
            if (document.hidden) {
                if (!IsPageHidden) Hide();
                IsPageHidden = true;
            }
            else {
                if (HasToClick) return;
                if (IsPageHidden) Show();
                IsPageHidden = false;
            }
        };
        const BlurEvent_2 = () => {
            if (!IsPageHidden) Hide();
            IsPageHidden = true;
        };
        const BlurEvent_3 = () => {
            if (HasToClick) return;
            if (IsPageHidden) Show();
            IsPageHidden = false;
        }
        const KeyDown = (event) => {
            if ((event.ctrlKey == true || event.metaKey == true) && event.keyCode == 83) {
                event.preventDefault();
            }
            if ((event.ctrlKey == true || event.metaKey == true) && event.code == 83) {
                event.preventDefault();
            }
            if ((event.ctrlKey == true || event.metaKey == true) && event.keyCode == 80) {
                event.preventDefault();
            }
            if ((event.ctrlKey == true || event.metaKey == true) && event.code == 80) {
                event.preventDefault();
            }
            if (event.key == 'PrintScreen') {
                HasToClick = true;
                if (!IsPageHidden) Hide();
                IsPageHidden = true;
            }
            // Prevent Keyboard Shortcuts for Inspect Element 
            // F12
            if (event.key == "F12") {
                event.preventDefault();
            }
            // Control+Shift+I
            if ((event.ctrlKey == true || event.metaKey == true) && (event.shiftKey == true || event.altKey == true) && event.key.toLowerCase() == "i") {
                event.preventDefault();
            }
            // Control+Shift+C
            if ((event.ctrlKey == true || event.metaKey == true) && (event.shiftKey == true || event.altKey == true) && event.key.toLowerCase() == "c") {
                event.preventDefault();
            }
            // Control+Shift+J
            if ((event.ctrlKey == true || event.metaKey == true) && (event.shiftKey == true || event.altKey == true) && event.key.toLowerCase() == "j") {
                event.preventDefault();
            }

            if ((event.ctrlKey == true || event.metaKey == true) && event.shiftKey == true) {
                HasToClick = true;
                if (!IsPageHidden) Hide();
                IsPageHidden = true;
                CantClick = true;
            }
        };
        const KeyUp = (event) => {
            // This has issues due to the browser asking for clipboard sometimes.
            // if (event.key == 'PrintScreen') {
            //     navigator.clipboard.writeText('');
            // }
            if (event.ctrlKey == true || event.metaKey == true || event.shiftKey == true) {
                setTimeout(() => { CantClick = false; }, 100)
            }
        };
        document.addEventListener('keydown', KeyDown);
        document.addEventListener('keyup', KeyUp);
        window.addEventListener('blur', BlurEvent_2);
        window.addEventListener('focus', BlurEvent_3);
        _DIVC.addEventListener('click', (ev) => {
            if (!CantClick && HasToClick && IsPageHidden) {
                Show();
                IsPageHidden = false;
            }
        });

        if (IsPageHidden) Hide();
        _DIVC.style.display = "block";
        const CssText = _DIVC.style.cssText;
        _IntervalCheckID = setInterval(() => {
            if (IsPageHidden && CssText != _DIVC.style.cssText) {
                _DIVC.style.cssText = CssText;
            }
            if (IsPageHidden && !document.contains(_DIVC)) {
                document.body.appendChild(_DIVC);
            }
        }, 50)
        if (!IsPageHidden) _DIVC.style.display = "none";
        _CallNonEvents = () => {
            document.removeEventListener('keydown', KeyDown);
            document.removeEventListener('keyup', KeyUp);
            window.removeEventListener('blur', BlurEvent_2);
            window.removeEventListener('focus', BlurEvent_3);
        };
        console.log("[WebGLScreenshotPreventer] On, blur element created and shortcut preventer activated");
    },

    DeapplyProtection__deps: ["PROTECTIVE_CONTENT", "IntervalCheckID", "DIVC", "DIVLogo", "CallNonEvents"],
    DeapplyProtection: function () {
        if (!_PROTECTIVE_CONTENT) return;
        _PROTECTIVE_CONTENT = false;
        _DIVC.remove();
        _DIVC = null;
        _DIVLogo.remove();
        _DIVLogo = null;
        clearInterval(_IntervalCheckID);
        _IntervalCheckID = -1;
        _CallNonEvents();
        _CallNonEvents = null;
        console.log("[WebGLScreenshotPreventer] Off, blur element deleted and shortcut preventer deactivated");
    }
}
autoAddDeps(ScreenShotPreventer, 'PROTECTIVE_CONTENT');
autoAddDeps(ScreenShotPreventer, 'IntervalCheckID');
autoAddDeps(ScreenShotPreventer, 'DIVC');
autoAddDeps(ScreenShotPreventer, 'DIVLogo');
autoAddDeps(ScreenShotPreventer, 'CallNonEvents');
autoAddDeps(ScreenShotPreventer, 'IsBlurredBackground');
mergeInto(LibraryManager.library, ScreenShotPreventer);