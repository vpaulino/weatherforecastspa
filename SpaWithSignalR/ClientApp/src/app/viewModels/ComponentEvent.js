"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ComponentState;
(function (ComponentState) {
    ComponentState[ComponentState["idle"] = 0] = "idle";
    ComponentState[ComponentState["information"] = 1] = "information";
    ComponentState[ComponentState["success"] = 2] = "success";
    ComponentState[ComponentState["warning"] = 3] = "warning";
    ComponentState[ComponentState["error"] = 4] = "error";
})(ComponentState = exports.ComponentState || (exports.ComponentState = {}));
var ComponentEvent = /** @class */ (function () {
    function ComponentEvent(eventId, status, message, details) {
        this.eventId = eventId;
        this.status = status;
        this.message = message;
        this.details = details;
    }
    return ComponentEvent;
}());
exports.ComponentEvent = ComponentEvent;
//# sourceMappingURL=componentEvent.js.map