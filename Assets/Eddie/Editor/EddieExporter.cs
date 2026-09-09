// Este script só existe no Editor e não é incluído em builds de produção.
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Eddie.Editor
{
    /// <summary>
    /// Utilitário de exportação do pacote Eddie.
    /// Acesse via menu: Eddie > Export Package
    /// </summary>
    public static class EddieExporter
    {
        private const string PackageFolder = "Assets/Eddie";
        private const string OutputFileName = "Eddie_Package.unitypackage";

        [MenuItem("Eddie/Export .unitypackage")]
        public static void ExportPackage()
        {
            string outputPath = Path.Combine(
                Application.dataPath,
                "..",
                OutputFileName
            );

            AssetDatabase.ExportPackage(
                PackageFolder,
                outputPath,
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies
            );

            Debug.Log($"[Eddie] Pacote exportado com sucesso em:\n{Path.GetFullPath(outputPath)}");
            EditorUtility.RevealInFinder(Path.GetFullPath(outputPath));
        }

        [MenuItem("Eddie/About Eddie Package")]
        public static void About()
        {
            EditorUtility.DisplayDialog(
                "Eddie — Interactive Character",
                "Versão: 1.0.0\nAutor: Sinergia Educação\nPacote: com.SinergiaEducacao.eddie\n\nUse 'Eddie > Export .unitypackage' para gerar o arquivo de distribuição.",
                "OK"
            );
        }
    }
}
#endif
