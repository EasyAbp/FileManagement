$(function () {

    var l = abp.localization.getResource('EasyAbpFileManagement');

    var service = easyAbp.fileManagement.files.file;
    var createDirectoryModal = new abp.ModalManager(abp.appPath + 'FileManagement/Files/File/CreateDirectoryModal');
    var uploadModal = new abp.ModalManager(abp.appPath + 'FileManagement/Files/File/UploadModal');
    var renameModal = new abp.ModalManager(abp.appPath + 'FileManagement/Files/File/RenameModal');
    var reUploadModal = new abp.ModalManager(abp.appPath + 'FileManagement/Files/File/ReUploadModal');
    var moveModal = new abp.ModalManager(abp.appPath + 'FileManagement/Files/File/MoveModal');

    var dataTable = $('#FileTable').DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: true,
        searching: false,
        autoWidth: false,
        scrollCollapse: true,
        ajax: abp.libs.datatables.createAjax(service.getList, function () {
            return { fileContainerName: fileContainerName, ownerUserId: ownerUserId, parentId: parentId };
        }),
        columnDefs: [
            {
                rowAction: {
                    items:
                        [
                            {
                                text: l('Download'),
                                visible: function (data) {
                                    return data.fileType === 1 && abp.auth.isGranted('EasyAbp.FileManagement.File.GetDownloadInfo')
                                },
                                action: function (data) {
                                    easyAbp.fileManagement.files.file.getDownloadInfo(data.record.id, {
                                        success: function (info) {
                                            const a = document.createElement("a");
                                            a.download = info.expectedFileName;
                                            a.href = info.downloadUrl;
                                            a.click();
                                            a.remove();
                                        }
                                    })
                                }
                            },
                            {
                                text: l('Rename'),
                                visible: abp.auth.isGranted('EasyAbp.FileManagement.File.Update'),
                                action: function (data) {
                                    renameModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('ReUpload'),
                                visible: function (data) {
                                    return data.fileType === 1 && abp.auth.isGranted('EasyAbp.FileManagement.File.Update')
                                },
                                action: function (data) {
                                    reUploadModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Move'),
                                visible: abp.auth.isGranted('EasyAbp.FileManagement.File.Move'),
                                action: function (data) {
                                    moveModal.open({ id: data.record.id });
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
            {
                data: "fileName",
                render: function (data, type, row) {
                    if (row.fileType === 1) {
                        return `<i class="file-icon fa fa-file fa-fw" aria-hidden="true"></i>${data}`;
                    }
                    if (row.fileType === 2) {
                        return `<a href="${getDirectoryUrl(fileContainerName, ownerUserId, row.id)}"><i class="directory-icon fa fa-folder fa-fw" aria-hidden="true"></i>${data}</a>`;
                    }
                }
            },
            {
                data: "byteSize",
                render: function (data, type, row) {
                    return humanFileSize(data, true);
                }
            },
        ]
    }));

    createDirectoryModal.onResult(function () {
        dataTable.ajax.reload();
    });

    uploadModal.onResult(function () {
        dataTable.ajax.reload();
    });

    renameModal.onResult(function () {
        dataTable.ajax.reload();
    });

    reUploadModal.onResult(function () {
        dataTable.ajax.reload();
    });

    moveModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $('#CreateDirectoryButton').click(function (e) {
        e.preventDefault();
        createDirectoryModal.open({ fileContainerName: fileContainerName, ownerUserId: ownerUserId, parentId: parentId });
    });

    $('#UploadFileButton').click(function (e) {
        e.preventDefault();
        uploadModal.open({ fileContainerName: fileContainerName, ownerUserId: ownerUserId, parentId: parentId });
    });
    
    $('#search-button').click(function (e) {
        e.preventDefault();
        var fileContainerName = $('#ViewModel_FileContainerName').val()
        var ownerUserId = $('#ViewModel_OwnerUserId').val()

        document.location.href = getDirectoryUrl(fileContainerName, ownerUserId, null);
    })

    function getDirectoryUrl(fileContainerName, ownerUserId, parentId) {
        return document.location.origin + document.location.pathname + '?fileContainerName=' + fileContainerName + '&ownerUserId=' + ownerUserId + '&parentId=' + parentId
    }

    function humanFileSize(bytes, si = false, dp = 1) {
        const thresh = si ? 1000 : 1024;

        if (Math.abs(bytes) < thresh) {
            return bytes + ' B';
        }

        const units = si
            ? ['kB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB']
            : ['KiB', 'MiB', 'GiB', 'TiB', 'PiB', 'EiB', 'ZiB', 'YiB'];
        let u = -1;
        const r = 10**dp;

        do {
            bytes /= thresh;
            ++u;
        } while (Math.round(Math.abs(bytes) * r) / r >= thresh && u < units.length - 1);


        return bytes.toFixed(dp) + ' ' + units[u];
    }
});