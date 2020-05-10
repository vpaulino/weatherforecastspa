"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ImageViewModel = /** @class */ (function () {
    function ImageViewModel(file) {
        var _this = this;
        var reader = new FileReader();
        reader.readAsDataURL(file); // read file as data url
        reader.onload = function (event) {
            _this.src = reader.result;
        };
        this.file = file;
    }
    return ImageViewModel;
}());
exports.ImageViewModel = ImageViewModel;
//# sourceMappingURL=ImageViewModel.js.map