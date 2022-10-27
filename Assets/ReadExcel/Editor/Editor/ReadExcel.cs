#pragma warning disable 0219

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEditor;
using UnityEngine;

public class ReadExcel : EditorWindow
{
    private readonly Dictionary<string, List<DataInfo>> dataTitleDic = new Dictionary<string, List<DataInfo>>(); //titledata
    private readonly ExcelFile excelFile = new ExcelFile(); //exceldata
    private readonly List<SheetInfo> sheetList = new List<SheetInfo>();
    private Dictionary<string, Vector2> scrollVeiwDic = new Dictionary<string, Vector2>();
    private string scriptName = String.Empty;
    private bool isSeparated;
    private void OnGUI()
    {
        GUILayout.Label("This My  Read Excel Window", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal("Box");
        GUILayout.Label("FileName", GUILayout.Width(70));
        GUILayout.Label(excelFile.fileName);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("Box");
        GUILayout.Label("FilePath", GUILayout.Width(70));
        GUILayout.Label(excelFile.filePath);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal("Box");
        GUILayout.Label("ScriptName", GUILayout.Width(100));
        scriptName = GUILayout.TextField(scriptName);
        EditorPrefs.SetString(excelFile.filePath + excelFile.fileName + "ScriptName", scriptName);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);


        if (GUILayout.Button("Create"))
        {

            if (IsEmptyEnable())
            {
                EditorUtility.DisplayDialog("Error", "No Data Is Seclected!", "OK");
                return;
            }

            CreateEntity();
            CreatExport();

            Close();
            AssetDatabase.ImportAsset(excelFile.filePath);
            AssetDatabase.Refresh(ImportAssetOptions.DontDownloadFromCacheServer);
            EditorUtility.DisplayDialog("Tips", "Complete!\nYou Need Click The Excel \"Reimprot\" Button", "OK");
        }

        #region show sheet list

        GUILayout.Space(10);

        GUILayout.BeginHorizontal("Box");
        isSeparated = GUILayout.Toggle(isSeparated, "Is Separate The Sheet?");
        EditorPrefs.SetBool(excelFile.filePath + excelFile.fileName + "isSeparated", isSeparated);
        GUILayout.EndHorizontal();

        GUILayout.Label("This  Excel Sheet List", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("Box");
        for (int j = 0, jMax = sheetList.Count; j < jMax; j++)
        {
            GUILayout.BeginHorizontal("Box");
            sheetList[j].isEnable = GUILayout.Toggle(sheetList[j].isEnable, "Open", GUILayout.Width(70));
            EditorPrefs.SetBool(excelFile.filePath + sheetList[j].name + "SheetEnable", sheetList[j].isEnable);
            GUILayout.Label(sheetList[j].name);
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
        }
        EditorGUILayout.EndVertical();

        #endregion

        foreach (var key in sheetList)
        {
            if (!dataTitleDic.ContainsKey(key.name))
            {
                continue;
            }
            //set show flase
            if (!key.isEnable)
            {
                continue;
            }
            var dataTitleList = dataTitleDic[key.name];

            //show data title list
            GUILayout.Space(10);
            var format = string.Format("This {0} Sheet data title List", key.name);
            key.isEnable = EditorGUILayout.BeginToggleGroup(format, key.isEnable);
            EditorGUILayout.BeginVertical("Box");
            scrollVeiwDic[key.name] = EditorGUILayout.BeginScrollView(scrollVeiwDic[key.name]);
            #region header

            GUILayout.BeginHorizontal();
            GUILayout.Label("ENABLE", GUILayout.Width(70));
            GUILayout.Label("ISLIST", GUILayout.Width(60));
            GUILayout.Label("TPYE", GUILayout.Width(120));
            GUILayout.Space(10);
            GUILayout.Label("DESCRIBLE", GUILayout.Width(250));
            GUILayout.Label("PROPERTY NAME");
            GUILayout.EndHorizontal();

            #endregion

            #region show sheet data

            for (int i = 0, iMax = dataTitleList.Count; i < iMax; i++)
            {
                EditorGUILayout.BeginHorizontal("Box");
                dataTitleList[i].isEnable = GUILayout.Toggle(dataTitleList[i].isEnable, "", GUILayout.Width(60));
                EditorPrefs.SetBool(excelFile.filePath + dataTitleList[i].titleName + "isEnable",
                    dataTitleList[i].isEnable);
                GUILayout.Toggle(dataTitleList[i].isArray, "", GUILayout.Width(60));
                dataTitleList[i].type =
                    (ValueType)EditorGUILayout.EnumPopup(dataTitleList[i].type, GUILayout.Width(120));
                if (EditorPrefs.GetInt(excelFile.filePath + key.name+ ".type." +dataTitleList[i].titleName,5000) < 5000)
                {
                    EditorPrefs.SetInt(excelFile.filePath + key.name+ ".type." + dataTitleList[i].titleName,
                                       (int)dataTitleList[i].type);
                }
                // Debug.Log("EditorPrefs.GetInt(excelFile.filePath + key.name+ .type. + dataTitleList[i].titleName:"+EditorPrefs.GetInt(excelFile.filePath + key.name+ ".type." + dataTitleList[i].titleName));
                GUILayout.Space(10);
                GUILayout.Label(dataTitleList[i].desc, GUILayout.Width(250));
                dataTitleList[i].titleName = GUILayout.TextField(dataTitleList[i].titleName);

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(2);
            }

            #endregion

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndToggleGroup();
        }

    }

    [MenuItem("Assets/Read Excel")]
    public static void ReadExcelMethod()
    {
        var myWindow = GetWindow<ReadExcel>();

        foreach (var obj in Selection.objects)
        {
            myWindow.excelFile.filePath = AssetDatabase.GetAssetPath(obj);
            myWindow.excelFile.fileName = Path.GetFileName(myWindow.excelFile.filePath);
            myWindow.scriptName =
           EditorPrefs.GetString(myWindow.excelFile.filePath + myWindow.excelFile.fileName + "ScriptName",
               Path.GetFileNameWithoutExtension(myWindow.excelFile.filePath));
            myWindow.isSeparated =
                EditorPrefs.GetBool(myWindow.excelFile.filePath + myWindow.excelFile.fileName + "isSeparated",
                   false);
            using (var stream = File.Open(myWindow.excelFile.filePath, FileMode.Open, FileAccess.Read))
            {
                //get excl data 
                IWorkbook book = null;
                if (Path.GetExtension(myWindow.excelFile.fileName) == ".xls")
                {
                    book = new HSSFWorkbook(stream);
                }
                else
                {
                    book = new XSSFWorkbook(stream);
                }
                //get excl sheet
                for (int j = 0, jMax = book.NumberOfSheets; j < jMax; j++)
                {

                    var sheet = book.GetSheetAt(j);
                    var sheetInfo = new SheetInfo();
                    sheetInfo.name = sheet.SheetName;
                    sheetInfo.isEnable =
                        EditorPrefs.GetBool(myWindow.excelFile.filePath + sheetInfo.name + "SheetEnable", true);
                    myWindow.sheetList.Add(sheetInfo);
                    myWindow.scrollVeiwDic.Add(sheetInfo.name, Vector2.zero);
                    // set datatitle list
                    var sheetOne = book.GetSheetAt(j);
                    var title = sheetOne.GetRow(0); //first row is property
                    if (title == null)
                    {
                        return;
                    }
                    var descRow = sheetOne.GetRow(1); // second row is describe
                    var dataRow = sheetOne.GetRow(2); //third row is data begain
                    var dataList = new List<DataInfo>();
                    for (int i = 0, iMax = title.Cells.Count; i < iMax; i++)
                    {

                        if (!string.IsNullOrEmpty(title.Cells[i].ToString()))
                        {
                            var data = new DataInfo();
                            data.titleName = title.Cells[i].ToString();
                            data.desc = descRow.Cells[i].ToString();
                            data.isArray = data.titleName.Contains("[]");
                            if (data.isArray)
                            {
                                data.titleName = data.titleName.Replace("[]", "");
                            }
                            data.isEnable = EditorPrefs.GetBool(myWindow.excelFile.filePath + data.titleName + "isEnable",
                                true);
                            var cell = dataRow.Cells[i];

                            data = SetDataType(cell, data, myWindow);
                            dataList.Add(data);
                        }

                    }
                    myWindow.dataTitleDic.Add(sheetOne.SheetName, dataList);
                }
            }
        }
    }

    /// <summary>
    ///     set data type auto
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="data"></param>
    /// <param name="myWindow"></param>
    /// <returns></returns>
    private static DataInfo SetDataType(ICell cell, DataInfo data, ReadExcel myWindow)
    {
        // Debug.Log("<color=red>"+myWindow.excelFile.filePath+cell.Sheet.SheetName + ".type." + data.titleName+"</color>");
        if (cell.CellType != CellType.Unknown && cell.CellType != CellType.Blank)
        {
            if (EditorPrefs.GetInt(myWindow.excelFile.filePath+cell.Sheet.SheetName + ".type." + data.titleName, 20) != 20)
            {
                data.type = (ValueType)EditorPrefs.GetInt(myWindow.excelFile.filePath+cell.Sheet.SheetName + ".type." + data.titleName);
                return data;
            }
            data.type = ValueType.STRING;
            EditorPrefs.SetInt(myWindow.excelFile.filePath+cell.Sheet.SheetName + ".type." + data.titleName, (int)ValueType.STRING);

            try
            {
                // Debug.Log("int:" + cell.ToString());
                int.Parse(cell.ToString());
                data.type = ValueType.INT;
                EditorPrefs.SetInt(myWindow.excelFile.filePath+cell.Sheet.SheetName + ".type." + data.titleName, (int)ValueType.INT);
                if (data.isArray)
                {
                    try
                    {
                        var datas = cell.ToString().Split(',');
                        int.Parse(datas[0]);
                        data.type = ValueType.INT;
                        EditorPrefs.SetInt(myWindow.excelFile.filePath+cell.Sheet.SheetName + ".type." + data.titleName, (int)ValueType.INT);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }

            try
            {
                bool.Parse(cell.ToString());
                data.type = ValueType.BOOL;
                EditorPrefs.SetInt(myWindow.excelFile.filePath+cell.Sheet.SheetName + ".type." + data.titleName, (int)ValueType.BOOL);
            }
            catch
            {
            }
        }
        return data;
    }

    /// <summary>
    /// check sheet is all false
    /// </summary>
    /// <returns></returns>
    private bool IsEmptyEnable()
    {
        for (int i = 0, iMax = sheetList.Count; i < iMax; i++)
        {
            if (sheetList[i].isEnable)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// create script obj entity
    /// </summary>
    /// <returns></returns>
    private void CreateEntity()
    {
        var copyFilePath = isSeparated
            ? "Assets/ReadExcel/Editor/Editor/EntityTemplate2.txt"
            : "Assets/ReadExcel/Editor/Editor/EntityTemplate.txt";

        Directory.CreateDirectory("Assets/ReadExcel/Config/Res/DataEntity/");
        if (isSeparated)
        {
            foreach (var sheet in sheetList)
            {
                if (!sheet.isEnable)
                {
                    continue;
                }
                var copyScript = File.ReadAllText(copyFilePath);
                var sb = new StringBuilder(copyScript);
                var SN = scriptName + "_" + sheet.name;
                var param = CreatEntityParam(sheet.name);
                sb.Replace("$Types$", param.ToString());
                sb.Replace("$ExcelData$", SN);
                File.WriteAllText("Assets/ReadExcel/Config/Res/DataEntity/" + SN + ".cs",
                    sb.ToString());
            }
        }
        else
        {
            var sheetEnum = new StringBuilder();
            foreach (var item in sheetList)
            {
                if (item.isEnable)
                {
                    sheetEnum.AppendLine(item.name + ",");
                }
            }
            foreach (var sheet in sheetList)
            {
                if (sheet.isEnable)
                {
                    var copyScript = File.ReadAllText(copyFilePath);
                    var sb = new StringBuilder(copyScript);
                    var param = CreatEntityParam(sheet.name);
                    sb.Replace("$Types$", param.ToString());
                    sb.Replace("$ExcelData$", scriptName);
                    sb.Replace("$SheetName$", sheetEnum.ToString());
                    File.WriteAllText("Assets/ReadExcel/Config/Res/DataEntity/" + scriptName + ".cs",
                        sb.ToString());
                    return;
                }
            }
        }
    }

    private StringBuilder CreatEntityParam(string sheetName)
    {
        var param = new StringBuilder();
        foreach (var item in dataTitleDic[sheetName])
        {
            if (!item.isEnable)
            {
                continue;
            }
            if (item.isArray)
            {
                param.AppendLine();
                param.AppendLine("        /// <summary>");
                param.AppendFormat("        /// {0}", item.desc);
                param.AppendLine();
                param.AppendLine("        /// </summary>");
                param.AppendFormat("        public  List<{0}> {1};", item.type.ToString().ToLower(), item.titleName);
                param.AppendLine();
            }
            else
            {
                param.AppendLine();
                param.AppendLine("        /// <summary>");
                param.AppendFormat("        /// {0}", item.desc);
                param.AppendLine();
                param.AppendLine("        /// </summary>");
                param.AppendFormat("        public  {0} {1};", item.type.ToString().ToLower(), item.titleName);
                param.AppendLine();
            }
        }
        return param;
    }

    /// <summary>
    ///     create scriptobjtect export
    /// </summary>
    private void CreatExport()
    {
        var copyFilePath = isSeparated
            ? "Assets/ReadExcel/Editor/Editor/ExportTemplate2.txt"
            : "Assets/ReadExcel/Editor/Editor/ExportTemplate.txt";

        Directory.CreateDirectory("Assets/ReadExcel/Editor/Importer/");
        if (isSeparated)
        {
            foreach (var sheet in sheetList)
            {
                if (!sheet.isEnable)
                {
                    continue;
                }
                var copyScript = File.ReadAllText(copyFilePath);
                var SN = scriptName + "_" + sheet.name;
                var exportAssetDirectry = "Assets/ReadExcel/Config/Res/DataConfig/" + SN;
                var builder = CreatImporterBuilder(sheet.name);

                copyScript = copyScript.Replace("$IMPORT_PATH$", excelFile.filePath);
                copyScript = copyScript.Replace("$SheetName$", sheet.name);
                copyScript = copyScript.Replace("$ExportAssetDirectry$", exportAssetDirectry);
                copyScript = copyScript.Replace("$ExcelData$", SN);
                copyScript = copyScript.Replace("$EXPORT_DATA$", builder.ToString());
                copyScript = copyScript.Replace("$ExportTemplate$", SN + "_Importer");

                File.WriteAllText("Assets/ReadExcel/Editor/Importer/" + SN + "_Importer" + ".cs", copyScript);
            }
        }
        else
        {
            foreach (var sheet in sheetList)
            {
                if (sheet.isEnable)
                {
                    var copyScript = File.ReadAllText(copyFilePath);
                    var exportAssetDirectry = "Assets/ReadExcel/Config/Res/DataConfig/" + scriptName;
                    var builder = CreatImporterBuilder(sheet.name);
                    copyScript = copyScript.Replace("$IMPORT_PATH$", excelFile.filePath);
                    var sb = new StringBuilder();
                    foreach (var item in sheetList)
                    {
                        if (item.isEnable)
                        {
                            sb.Append("\"" + item.name + "\",");
                        }
                    }
                    copyScript = copyScript.Replace("$SheetList$", sb.ToString());
                    copyScript = copyScript.Replace("$EXPORT_PATH$", exportAssetDirectry);
                    copyScript = copyScript.Replace("$ExcelData$", scriptName);
                    copyScript = copyScript.Replace("$EXPORT_DATA$", builder.ToString());
                    copyScript = copyScript.Replace("$ExportTemplate$", scriptName + "_Importer");

                    File.WriteAllText("Assets/ReadExcel/Editor/Importer/" + scriptName + "_Importer" + ".cs",
                        copyScript);
                    return;
                }
            }
        }
    }

    private StringBuilder CreatImporterBuilder(string sheetName)
    {
        var builder = new StringBuilder();
        var tab = "                        ";
        var rowCount = 0;
        var ListCode =
            "cell = row.GetCell({0});var {1}temp =" +
            " (cell == null ? \"\" : cell.StringCellValue);string [] {1}" +
            "temps={1}temp.Split(\',\');for(int index =0," +
            "iMax={1}temps.Length;index<iMax;index++)";
        foreach (var item in dataTitleDic[sheetName])
        {
            #region set data

            if (!item.isEnable)
                continue;
            if (item.isArray)
            {
                builder.AppendLine();
                switch (item.type)
                {
                    case ValueType.BOOL:
                        builder.AppendFormat(tab + "p.{0} = new List<bool>();", item.titleName);
                        var boolString =
                            "{{bool val=false; if({1}temps[index]==\"TRUE\"){val=true;}else{val=flase;}p.{1}.Add(val);}}";
                        builder.AppendFormat(tab + ListCode + boolString, rowCount, item.titleName);

                        break;
                    case ValueType.DOUBLE:
                        builder.AppendFormat(tab + "p.{0} = new List<double>();", item.titleName);
                        var doubleString = "{{double val=0; val=double.Parse({1}temps[index]);p.{1}.Add(val);}}";
                        builder.AppendFormat(tab + ListCode + doubleString, rowCount, item.titleName);
                        break;
                    case ValueType.INT:
                        builder.AppendFormat(tab + "p.{0} = new List<int>();", item.titleName);
                        var intString = "{{int val=0; val=int.Parse({1}temps[index]);p.{1}.Add(val);}}";
                        builder.AppendFormat(tab + ListCode + intString, rowCount, item.titleName);
                        break;
                    case ValueType.FLOAT:
                        builder.AppendFormat(tab + "p.{0} = new List<float>();", item.titleName);
                        var floatString = "{{float val=0; val=float.Parse({1}temps[index]);p.{1}.Add(val);}}";
                        builder.AppendFormat(tab + ListCode + floatString, rowCount, item.titleName);
                        break;
                    case ValueType.STRING:
                        builder.AppendFormat(tab + "p.{0} = new List<string>();", item.titleName);
                        var str = "{{string val={1}temps[index];p.{1}.Add(val);}}";
                        builder.AppendFormat(tab + ListCode + str, rowCount, item.titleName);
                        break;
                }
            }
            else
            {
                builder.AppendLine();
                switch (item.type)
                {
                    case ValueType.BOOL:
                        builder.AppendFormat(
                            tab +
                            "cell = row.GetCell({1}); p.{0} = (cell == null ? false : cell.BooleanCellValue);",
                            item.titleName, rowCount);
                        break;
                    case ValueType.DOUBLE:
                        builder.AppendFormat(
                            tab +
                            "cell = row.GetCell({1}); p.{0} = (cell == null ? 0.0 : cell.NumericCellValue);",
                            item.titleName, rowCount);
                        break;
                    case ValueType.INT:
                        builder.AppendFormat(
                            tab +
                            "cell = row.GetCell({1}); p.{0} = (int)(cell == null ? 0 : cell.NumericCellValue);",
                            item.titleName, rowCount);
                        break;
                    case ValueType.FLOAT:
                        builder.AppendFormat(
                            tab +
                            "cell = row.GetCell({1}); p.{0} = (float)(cell == null ? 0 : cell.NumericCellValue);",
                            item.titleName, rowCount);
                        break;
                    case ValueType.STRING:
                        builder.AppendFormat(
                            tab +
                            "cell = row.GetCell({1}); p.{0} = (cell == null ? \"\" : cell.StringCellValue);",
                            item.titleName, rowCount);
                        break;
                }
            }
            rowCount += 1;

            #endregion
        }
        return builder;
    }

    private class ExcelFile
    {
        public string fileName;
        public string filePath;
    }

    private class SheetInfo
    {
        public bool isEnable;
        public string name;
    }

    private class DataInfo
    {
        public string desc;
        public bool isArray;
        public bool isEnable;
        public string titleName;
        public ValueType type;
    }

    private enum ValueType
    {
        BOOL,
        STRING,
        INT,
        FLOAT,
        DOUBLE
    }

    public static void CreatDataInitCs()
    {
        var scriptObjs = Directory.GetFiles("Assets/ReadExcel/Config/Res/DataConfig/", "*.asset");
        var scriptEntity = Directory.GetFiles("Assets/ReadExcel/Config/Res/DataEntity/", "*.cs");
        for (int i = 0, iMax = scriptEntity.Length; i < iMax; i++)
        {
            scriptEntity[i] = Path.GetFileNameWithoutExtension(scriptEntity[i]);
        }
        var replaceSB = new StringBuilder();
        for (int i = 0, iMax = scriptObjs.Length; i < iMax; i++)
        {
            replaceSB.AppendLine(string.Format("AddressableManager.Instance.LoadSystemAsset<{0}>(\"{1}\").SetDic();",
                scriptEntity[i], Path.GetFileNameWithoutExtension(scriptObjs[i])));
        }
        var sb = new StringBuilder(File.ReadAllText("Assets/ReadExcel/Editor/Editor/ExcelDataInit.txt"));
        sb.Replace("$ResourcesScriptObject$", replaceSB.ToString());
        File.WriteAllText("Assets/ReadExcel/Config/Res/" + "ExcelDataInit" + ".cs", sb.ToString());
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }
}