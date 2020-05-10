"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ImageViewModel_1 = require("../viewModels/ImageViewModel");
var ImageDisplayViewModel = /** @class */ (function () {
    function ImageDisplayViewModel() {
    }
    ImageDisplayViewModel.prototype.addImageToDisplay = function (file) {
        this.imagesViewModels.push(new ImageViewModel_1.ImageViewModel(file));
    };
    return ImageDisplayViewModel;
}());
exports.ImageDisplayViewModel = ImageDisplayViewModel;
//# sourceMappingURL=ImageDisplayViewModel.js.map