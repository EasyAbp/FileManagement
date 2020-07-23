$(function () {

    var l = abp.localization.getResource('EasyAbpFileManagement');

    var service = easyAbp.fileManagement.files.file;
    var createModal = new abp.ModalManager(abp.appPath + 'FileManagement/Files/File/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'FileManagement/Files/File/EditModal');

    var dataTable = $('#FileTable').DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: true,
        searching: false,
        autoWidth: false,
        scrollCollapse: true,
        order: [[1, "asc"]],
        ajax: abp.libs.datatables.createAjax(service.getList),
        columnDefs: [
            {
                rowAction: {
                    items:
                        [
                            {
                                text: l('Edit'),
                                visible: abp.auth.isGranted('EasyAbp.FileManagement.File.Update'),
                                action: function (data) {
                                    editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Move'),
                                visible: abp.auth.isGranted('EasyAbp.FileManagement.File.Move'),
                                action: function (data) {
                                    editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Delete'),
                                visible: abp.auth.isGranted('EasyAbp.FileManagement.File.Delete'),
                                confirmMessage: function (data) {
                                    return l('FileDeletionConfirmationMessage', data.record.id);
                                },
                                action: function (data) {
                                        service.delete(data.record.id)
                                        .then(function () {
                                            abp.notify.info(l('SuccessfullyDeleted'));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                }
            },
            { data: "fileContainerName" },
            { data: "fileName" },
            { data: "filePath" },
            { data: "mimeType" },
            { data: "fileType" },
            { data: "subFilesQuantity" },
            { data: "byteSize" },
            { data: "hash" },
            { data: "code" },
            { data: "level" },
            { data: "parentId" },
            { data: "parent" },
            { data: "children" },
            { data: "displayName" },
        ]
    }));

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $('#NewFileButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });
});