mergeInto(LibraryManager.library, {
  ShowConfirm: function (msgPtr) {
    var msg = UTF8ToString(msgPtr);
    return confirm(msg) ? 1 : 0; // 1=OK, 0=Cancel
  }
});
