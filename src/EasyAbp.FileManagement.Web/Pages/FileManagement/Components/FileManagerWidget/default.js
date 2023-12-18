(function ($) {

    abp.widgets.FileManager = function ($widget) {

        var widgetManager = $widget.data('abp-widget-manager');
        var $fileManagerArea = $widget.find('.file-manager-area');

        function getFilters() {
            return {
                fileContainerName: $fileManagerArea.attr('data-file-container-name'),
                ownerUserId: $fileManagerArea.attr('data-owner-user-id'),
                parentId: $fileManagerArea.attr('data-parent-id'),
                grandparentId: $fileManagerArea.attr('data-grandparent-id')
            };
        }

        function initFileManagerTable(filters) {

            var fileContainerName = filters.fileContainerName;
            var ownerUserId = filters.ownerUserId;
            var parentId = filters.parentId;
            var grandparentId = filters.grandparentId;

            var canDeleteFile = toBoolAttrValue($fileManagerArea.attr('data-can-delete-file'));
            var canDeleteDirectory = toBoolAttrValue($fileManagerArea.attr('data-can-delete-directory'));
            var canMoveFile = toBoolAttrValue($fileManagerArea.attr('data-can-move-file'));
            var canMoveDirectory = toBoolAttrValue($fileManagerArea.attr('data-can-move-directory'));
            var canRenameFile = toBoolAttrValue($fileManagerArea.attr('data-can-rename-file'));
            var canRenameDirectory = toBoolAttrValue($fileManagerArea.attr('data-can-rename-directory'));
            var canDownloadFile = toBoolAttrValue($fileManagerArea.attr('data-can-download-file'));

            if (canDeleteFile === null) canDeleteFile = abp.auth.isGranted('EasyAbp.FileManagement.File.Delete');
            if (canDeleteDirectory === null) canDeleteDirectory = abp.auth.isGranted('EasyAbp.FileManagement.File.Delete');
            if (canMoveFile === null) canMoveFile = abp.auth.isGranted('EasyAbp.FileManagement.File.Move');
            if (canMoveDirectory === null) canMoveDirectory = abp.auth.isGranted('EasyAbp.FileManagement.File.Move');
            if (canRenameFile === null) canRenameFile = abp.auth.isGranted('EasyAbp.FileManagement.File.Update');
            if (canRenameDirectory === null) canRenameDirectory = abp.auth.isGranted('EasyAbp.FileManagement.File.Update');
            if (canDownloadFile === null) canDownloadFile = abp.auth.isGranted('EasyAbp.FileManagement.File.GetDownloadInfo');

            var l = abp.localization.getResource('EasyAbpFileManagement');

            var service = easyAbp.fileManagement.files.file;
            var createDirectoryModal = new abp.ModalManager(abp.appPath + 'FileManagement/Components/FileManagerWidget/CreateDirectoryModal');
            var uploadModal = new abp.ModalManager(abp.appPath + 'FileManagement/Components/FileManagerWidget/UploadModal');
            var renameModal = new abp.ModalManager(abp.appPath + 'FileManagement/Components/FileManagerWidget/RenameModal');
            var moveModal = new abp.ModalManager(abp.appPath + 'FileManagement/Components/FileManagerWidget/MoveModal');
            var detailModal = new abp.ModalManager(abp.appPath + 'FileManagement/Components/FileManagerWidget/DetailModal');

            var dataTable = $widget.find('.file-table').DataTable(abp.libs.datatables.normalizeConfiguration({
                processing: true,
                serverSide: true,
                paging: true,
                searching: false,
                autoWidth: false,
                scrollCollapse: true,
                ajax: abp.libs.datatables.createAjax(service.getList, function () {
                    return {fileContainerName: fileContainerName, ownerUserId: ownerUserId, parentId: parentId};
                }, function (result) {
                    if (parentId) {
                        result.items.unshift({ id: grandparentId, fileName: "[...]", byteSize: null, fileType: 1, lastModificationTime: null, creationTime: null })
                    }
                    return {
                        recordsTotal: result.totalCount,
                        recordsFiltered: result.items.length,
                        data: result.items
                    };
                }),
                columnDefs: [
                    {
                        rowAction: {
                            items:
                                [
                                    {
                                        text: l('Download'),
                                        visible: function (data) {
                                            return data.fileType === 2 && canDownloadFile && data.id !== grandparentId
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
                                        visible: function (data) {
                                            return (data.fileType === 1 ? canRenameDirectory : canRenameFile) && data.id !== grandparentId
                                        },
                                        action: function (data) {
                                            renameModal.open({id: data.record.id});
                                        }
                                    },
                                    {
                                        text: l('Move'),
                                        visible: function (data) {
                                            return (data.fileType === 1 ? canMoveDirectory : canMoveFile) && data.id !== grandparentId
                                        },
                                        action: function (data) {
                                            moveModal.open({id: data.record.id});
                                        }
                                    },
                                    {
                                        text: l('Delete'),
                                        visible: function (data) {
                                            return (data.fileType === 1 ? canDeleteDirectory : canDeleteFile) && data.id !== grandparentId
                                        },
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
                                    },
                                    {
                                        text: l('Details'),
                                        visible: function (data) {
                                            return data.id !== grandparentId
                                        },
                                        action: function (data) {
                                            detailModal.open({id: data.record.id});
                                        }
                                    },
                                ]
                        }
                    },
                    {
                        data: "fileName",
                        render: function (data, type, row) {
                            if (row.fileType === 1) {
                                return `<a class="dir-link" href="#" data-dir-id="${row.id}"><i class="directory-icon fa fa-folder fa-fw" aria-hidden="true"></i>${data}</a>`;
                            }
                            if (row.fileType === 2) {
                                return `<i class="file-icon fa fa-file fa-fw" aria-hidden="true"></i>${data}`;
                            }
                        }
                    },
                    {
                        data: "byteSize",
                        render: function (data, type, row) {
                            return humanFileSize(data, true);
                        }
                    },
                    {
                        data: "lastModificationTime",
                        render: function (data, type, row) {
                            return row.lastModificationTime || row.creationTime
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

            moveModal.onResult(function () {
                dataTable.ajax.reload();
            });

            $widget.find('.create-dir-btn').click(function (e) {
                e.preventDefault();
                createDirectoryModal.open({
                    fileContainerName: fileContainerName,
                    ownerUserId: ownerUserId,
                    parentId: parentId
                });
            });

            $widget.find('.upload-file-btn').click(function (e) {
                e.preventDefault();
                uploadModal.open({fileContainerName: fileContainerName, ownerUserId: ownerUserId, parentId: parentId});
            });

            $widget.on('click', '.dir-link', function (e) {
                e.preventDefault();
                const id = $(this).attr('data-dir-id');
                $fileManagerArea.attr('data-parent-id', id);
                widgetManager.refresh($widget);
            });

            function toBoolAttrValue(strValue) {
                if (strValue === 'True'){
                    return true;
                } else if (strValue === 'False') {
                    return false;
                } else {
                    return null;
                }
            }

            function humanFileSize(bytes, si = false, dp = 1) {
                const thresh = si ? 1000 : 1024;

                if (bytes === null) return ''

                if (Math.abs(bytes) < thresh) {
                    return bytes + ' B';
                }

                const units = si
                    ? ['kB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB']
                    : ['KiB', 'MiB', 'GiB', 'TiB', 'PiB', 'EiB', 'ZiB', 'YiB'];
                let u = -1;
                const r = Math.pow(10, dp);

                do {
                    bytes /= thresh;
                    ++u;
                } while (Math.round(Math.abs(bytes) * r) / r >= thresh && u < units.length - 1);


                return bytes.toFixed(dp) + ' ' + units[u];
            }
        }

        function init(filters) {
            initFileManagerTable(filters);
        }

        return {
            getFilters: getFilters,
            init: init
        };
    };
})(jQuery);