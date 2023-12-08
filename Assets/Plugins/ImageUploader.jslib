var UploadFilePlugin = {
  UploadFile: function(gameObjectName, methodName) {
    var gameObject = UTF8ToString(gameObjectName);
    var method = UTF8ToString(methodName);
    var unitycanvas = document.getElementById('unity-canvas');
    if(!document.getElementById('UploadFileInput'))
    {
      var fileInput = document.createElement('input');
      fileInput.setAttribute('type', 'file');
      fileInput.setAttribute('id', 'UploadFileInput');
      fileInput.setAttribute('accept', '.zip');
      fileInput.style.visibility = 'hidden';
      fileInput.style.display = 'none';
      fileInput.onclick = function (event)
      {
        this.value = null;
        var element = document.getElementById('UploadFileInput');
        element.parentNode.removeChild(element);
        unitycanvas.removeEventListener('click', OpenFileDialog, false);
      };
      fileInput.onchange = function (event)
      {
        if(event.target.value != null){
          SendMessage(gameObject, method, URL.createObjectURL(event.target.files[0]));
        }
      };
      document.body.appendChild(fileInput);
    }
    var OpenFileDialog = function()
    {
      document.getElementById('UploadFileInput').click();
    };
    unitycanvas.addEventListener('click', OpenFileDialog, false);
  }
};
mergeInto(LibraryManager.library, UploadFilePlugin);