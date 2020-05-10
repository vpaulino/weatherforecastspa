"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ImageViewModel_1 = require("../viewModels/ImageViewModel");
var UploadFileViewModel = /** @class */ (function () {
    function UploadFileViewModel() {
        this.images = [];
    }
    UploadFileViewModel.prototype.addFile = function (file) {
        this.images.push(new ImageViewModel_1.ImageViewModel(file));
    };
    UploadFileViewModel.prototype.clearAllFiles = function () {
        this.images = [];
    };
    UploadFileViewModel.prototype.filesAdded = function () {
        return this.images.length > 0;
    };
    UploadFileViewModel.prototype.getAllFiles = function () {
        return this.images;
    };
    return UploadFileViewModel;
}());
exports.UploadFileViewModel = UploadFileViewModel;
//# sourceMappingURL=UploadFileViewModel.js.map