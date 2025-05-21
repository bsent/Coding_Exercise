using Microsoft.VisualStudio.TestTools.UnitTesting;
using Exercise.Pages; // Required for IndexModel
using JobLib;         // Required for JobBundles (to check cleanup)
using System.Linq;    // Required for LINQ assertions

namespace JobTests
{
    [TestClass]
    public class IndexPageModelTests
    {
        private IndexModel _indexModel;

        [TestInitialize]
        public void TestInitialize()
        {
            // Reset static dictionary before each test to ensure a clean state,
            // as IndexModel.LoadJobBundlesIfNotPresent() only loads if it's empty.
            JobBundles.BundleDict.Clear(); 
            _indexModel = new IndexModel();
        }

        [TestMethod]
        public void OnGet_PopulatesPredefinedBundles()
        {
            // Act
            _indexModel.OnGet();

            // Assert
            Assert.IsNotNull(_indexModel.PredefinedBundles, "PredefinedBundles should not be null.");
            Assert.AreEqual(6, _indexModel.PredefinedBundles.Count, "PredefinedBundles should contain 6 bundles.");
            Assert.IsTrue(_indexModel.PredefinedBundles.ContainsKey("1"), "Bundle '1' should exist.");
            Assert.AreEqual("a=>", _indexModel.PredefinedBundles["1"]);
            Assert.IsTrue(_indexModel.PredefinedBundles.ContainsKey("6"), "Bundle '6' should exist.");
        }

        [TestMethod]
        public void OnPostExecuteBundle_WithValidBundle_ExecutesSuccessfully()
        {
            // Arrange
            _indexModel.OnGet(); // Load initial bundles
            _indexModel.SelectedBundle = "1";

            // Act
            _indexModel.OnPostExecuteBundle();

            // Assert
            Assert.IsNotNull(_indexModel.ExecutionResult, "ExecutionResult should not be null.");
            Assert.AreEqual("JobName : a", _indexModel.ExecutionResult.Trim());
        }

        [TestMethod]
        public void OnPostExecuteBundle_WithAnotherValidBundle_ExecutesSuccessfully()
        {
            // Arrange
            _indexModel.OnGet(); // Load initial bundles
            _indexModel.SelectedBundle = "2"; // Test with a different bundle

            // Act
            _indexModel.OnPostExecuteBundle();

            // Assert
            Assert.IsNotNull(_indexModel.ExecutionResult, "ExecutionResult should not be null.");
            // Based on JobBundles.BundleDict.Add("2", "a=>;b=>;c=>"); and Executor logic
            Assert.AreEqual("JobName : a JobName : b JobName : c", _indexModel.ExecutionResult.Trim());
        }

        [TestMethod]
        public void OnPostExecuteBundle_WithInvalidBundle_ReturnsError()
        {
            // Arrange
            _indexModel.OnGet(); // Load initial bundles
            _indexModel.SelectedBundle = "invalid_bundle_key";

            // Act
            _indexModel.OnPostExecuteBundle();

            // Assert
            Assert.IsNotNull(_indexModel.ExecutionResult, "ExecutionResult should not be null.");
            Assert.AreEqual("Error: Selected bundle not found or invalid.", _indexModel.ExecutionResult);
        }
        
        [TestMethod]
        public void OnPostExecuteBundle_WithNullBundle_ReturnsError()
        {
            // Arrange
            _indexModel.OnGet(); // Load initial bundles
            _indexModel.SelectedBundle = null;

            // Act
            _indexModel.OnPostExecuteBundle();

            // Assert
            Assert.IsNotNull(_indexModel.ExecutionResult, "ExecutionResult should not be null.");
            Assert.AreEqual("Error: Selected bundle not found or invalid.", _indexModel.ExecutionResult);
        }

        [TestMethod]
        public void OnPostExecuteCustomPattern_WithValidPattern_ExecutesSuccessfullyAndCleansUp()
        {
            // Arrange
            _indexModel.OnGet(); // Load initial bundles
            _indexModel.CustomPattern = "x=>;y=>x;z=>y";
            string tempCustomKey = "custom_temp_pattern";

            // Act
            _indexModel.OnPostExecuteCustomPattern();

            // Assert
            Assert.IsNotNull(_indexModel.ExecutionResult, "ExecutionResult should not be null.");
            Assert.AreEqual("JobName : x JobName : y JobName : z", _indexModel.ExecutionResult.Trim());
            Assert.IsFalse(JobBundles.BundleDict.ContainsKey(tempCustomKey), "Temporary custom bundle key should be removed after execution.");
            Assert.AreEqual(6, _indexModel.PredefinedBundles.Count, "PredefinedBundles count should remain unchanged after custom execution.");
        }

        [TestMethod]
        public void OnPostExecuteCustomPattern_WithEmptyPattern_ReturnsError()
        {
            // Arrange
            _indexModel.OnGet(); // Load initial bundles
            _indexModel.CustomPattern = "";

            // Act
            _indexModel.OnPostExecuteCustomPattern();

            // Assert
            Assert.IsNotNull(_indexModel.ExecutionResult, "ExecutionResult should not be null.");
            Assert.AreEqual("Error: Custom pattern cannot be empty.", _indexModel.ExecutionResult);
        }

        [TestMethod]
        public void OnPostExecuteCustomPattern_WithWhitespacePattern_ReturnsError()
        {
            // Arrange
            _indexModel.OnGet(); // Load initial bundles
            _indexModel.CustomPattern = "   ";

            // Act
            _indexModel.OnPostExecuteCustomPattern();

            // Assert
            Assert.IsNotNull(_indexModel.ExecutionResult, "ExecutionResult should not be null.");
            Assert.AreEqual("Error: Custom pattern cannot be empty.", _indexModel.ExecutionResult);
        }
        
        [TestMethod]
        public void OnPostExecuteCustomPattern_WithNullPattern_ReturnsError()
        {
            // Arrange
            _indexModel.OnGet(); // Load initial bundles
            _indexModel.CustomPattern = null;

            // Act
            _indexModel.OnPostExecuteCustomPattern();

            // Assert
            Assert.IsNotNull(_indexModel.ExecutionResult, "ExecutionResult should not be null.");
            Assert.AreEqual("Error: Custom pattern cannot be empty.", _indexModel.ExecutionResult);
        }
    }
}
