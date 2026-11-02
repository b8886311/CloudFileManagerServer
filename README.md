# Cloud File Manager

以 **Composite 模式**為核心的雲端檔案管理系統，從一份需求訪談稿出發，完成領域模型分析、
UML / ER 設計，並以 ASP.NET Core Web API 實作。

---

## 使用技術

| 項目 | 使用技術 |
|---|---|
| 語言 / 框架 | C# 12 / .NET 8 / ASP.NET Core Web API |
| 設計模式 | Composite、Command、Memento、Strategy、Singleton、Simple Factory |
| 序列化 | System.Xml.Linq (`XDocument`) |
| 文件 | UML 類別圖、ER Model（使用Drawio製作） |

---

## 專案背景

題目提供一份與虛擬客戶的需求訪談逐字稿，要求：

1. 從自然語言的訪談內容中，識別出實體、屬性與關係，建立領域模型
2. 產出 UML 類別圖與 ER Model
3. 實作核心邏輯（結構呈現、容量計算、副檔名搜尋、XML 輸出、走訪紀錄）
4. 進階功能（Bonus）：排序、剪下／複製／貼上、多重標籤、Undo / Redo


---

## 專案結構

```
CloudFileManagerServer/
├── FileNode.cs          # INode / BaseNode / DirectoryNode / FileNode（Composite）
├── Files.cs             # IFile / BaseFile / WordFile / ImageFile / TextFile
├── Tags.cs              # Tag / TagLevel
├── Algorithm.cs         # DFS / BFS 走訪、深拷貝、容量計算
├── CommentManager.cs    # ICommand / CommandManager（Undo / Redo）
├── EditCommands.cs      # Cut / Copy / Paste
├── SortCommands.cs      # SortCommand + 五個 IComparer（Strategy
├── TagCommands.cs       # TagCommand
├── TreeFormatter.cs     # 樹狀結構文字輸出
├── XmlSerializer.cs     # XML 序列化
├── FileManager.cs       # 應用服務層（Singleton）
└── Controllers/
└── FileSystemController.cs
```

---

## 快速開始

```bash
git clone https://github.com/b8886311/CloudFileManagerServer.git
cd CloudFileManagerServer
dotnet restore
dotnet run
```

啟動後開啟 `https://localhost:<PORT>/swagger` 即可操作全部 API。

**API操作細節可參考 `Solution.docx` 附圖實例**

---

## 設計文件

| 文件 | 檔名 | 說明 |
|---|---|---|
| UML 類別圖 | file_manager.png | API的進入點，包含各種方法的實現關係圖 |
| UML 類別圖 | node.png | 根據節點和檔案的關係產生關聯圖並繼承介面，讓最終程式只依賴介面不依賴實體類別 |
| UML 類別圖 | command_manager.png | 此為特殊功能類，根據用戶指令產生關聯圖並透過命令模式驅動程式，另支援undo/redo功能  |
| ER Model | er_model.png | 將類別繼承關係映射至ER Model |


**`以上文件均附圖檔，可至Drawio網站開啟圖檔驗證`**

---
