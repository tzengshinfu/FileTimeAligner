# FileTimeAligner

作用：修正檔案時間(最後修改時間)，來源可為檔案時間屬性或檔案名稱

平台：Windows 10 64 bit

用法：FileTimeAligner `<directory path>` `<options>`

| 參數                  | 說明                                                                                                |
| --------------------- | --------------------------------------------------------------------------------------------------- |
| directory path        | (必填)要對齊檔案時間的目錄路徑                                                                      |
| options               |                                                                                                     |
| -a, --all-directories | (選填)掃瞄子目錄                                                                                    |
| -t, --mime-type       | (選填)要對齊的檔案MIME類型，以分號區隔；預設為image;video                                           |
| -p, --property-keys   | (選填)作為對齊來源的時間屬性名稱，以分號區隔；預設為System.Photo.DateTaken;System.Media.DateEncoded |
| -f, --file-name       | (選填)檔案名稱若符合時間格式(yyyyMMddHHmmss)則優先使用                                              |
| -m, --minimum-date    | (選填)忽略小於此日期的時間屬性值；預設為1970/01/01                                                  |
