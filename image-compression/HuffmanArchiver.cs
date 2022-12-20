using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace image_compression
{
    public class HuffmanArchiver
    {
        /// <summary>
        /// Бинарное дерево.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [Serializable]
        public class BinaryTree<T>
        {
            public BinaryTreeNode<T> Root { get; set; }

            public BinaryTree(BinaryTreeNode<T> root) => Root = root;
        }

        /// <summary>
        /// Узел бинарного дерева.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [Serializable]
        public class BinaryTreeNode<T>
        {
            public BinaryTreeNode<T> LeftChild { get; set; }
            public BinaryTreeNode<T> RightChild { get; set; }
            public T Value { get; set; }

            public BinaryTreeNode(T value) => Value = value;

            public BinaryTreeNode(T value, BinaryTreeNode<T> leftChild, BinaryTreeNode<T> rightChild)
            {
                Value = value;
                LeftChild = leftChild;
                RightChild = rightChild;
            }
        }

        private const string UncodedImageFile = "uncodedImage.binary";
        private const string EncodedImageFile = "encodedImage.binary";
        private const string DecodedImageFile = "decodedImage.binary";

        /// <summary>
        /// Запись матрицы в файл.
        /// </summary>
        /// <param name="matrix"></param>
        private static void WriteToABinaryFile(ComplexMatrix matrix)
        {
            using var writer = new StreamWriter(File.Open(UncodedImageFile, FileMode.Create), Encoding.Default);
            writer.Write(matrix.Width + " " + matrix.Height + " ");

            for (var i = 0; i < matrix.Width; i++)
            for (var j = 0; j < matrix.Height; j++)
                writer.Write((int)matrix.Matrix[i][j].Real + " ");
        }

        /// <summary>
        /// Создание таблицы: символ - вероятность.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        private static Dictionary<char, double> CreateProbabilityTable()
        {
            if (!File.Exists(UncodedImageFile))
                throw new FileNotFoundException();

            var symbolCount = 0;
            var first = new Dictionary<char, int>();
            var second = new Dictionary<char, double>();

            using (var reader = new StreamReader(File.Open(UncodedImageFile, FileMode.OpenOrCreate), Encoding.Default))
            {
                int temp;
                while ((temp = reader.Read()) > -1)
                {
                    symbolCount++;
                    if (!first.ContainsKey((char)temp))
                        first.Add((char)temp, 1);
                    else first[(char)temp]++;
                }
            }

            first.ToList().ForEach(i => second.Add(i.Key, (double)i.Value / symbolCount));

            return second;
        }

        /// <summary>
        /// Создаёт бинарное дерево Хаффмана.
        /// </summary>
        /// <param name="symbolsTable"></param>
        /// <returns></returns>
        private static BinaryTree<string> CreateHuffmanTree(Dictionary<char, double> symbolsTable)
        {
            var binaryTree = new Dictionary<string, double>();
            symbolsTable.OrderBy(p => p.Value).ToList().ForEach(i => binaryTree.Add(i.Key.ToString(), i.Value));

            var nodeList = new Dictionary<string, BinaryTreeNode<string>>();
            binaryTree.Keys.ToList().ForEach(i => nodeList.Add(i, new BinaryTreeNode<string>(i)));

            while (binaryTree.Count > 1)
            {
                // Отбирает пару ключей с наименьшей вероятностью.
                var helpmass = binaryTree.Take(2).ToArray();

                // Скрепляет эту пару ключей, тем самым создаёт узел.
                var leftKey = helpmass[1].Key;
                var rightKey = helpmass[0].Key;
                var concatKey = string.Concat(leftKey, rightKey);

                var leftValue = helpmass[1].Value;
                var rigthValue = helpmass[0].Value;

                binaryTree.Remove(leftKey);
                binaryTree.Remove(rightKey);
                binaryTree.Add(concatKey, leftValue + rigthValue);

                nodeList.TryGetValue(leftKey, out var leftChild);
                nodeList.TryGetValue(rightKey, out var rightChild);
                nodeList.Remove(leftChild.Value);
                nodeList.Remove(rightChild.Value);
                nodeList.Add(concatKey, new BinaryTreeNode<string>(concatKey, leftChild, rightChild));
            }

            nodeList.TryGetValue(binaryTree.ElementAt(0).Key, out var root);

            return new BinaryTree<string>(root);
        }

        /// <summary>
        /// Создание кодовой таблицы.
        /// </summary>
        /// <param name="huffmanTree"></param>
        /// <param name="symbols"></param>
        /// <returns></returns>
        private static Dictionary<char, string> CreateCodeTable(BinaryTree<string> huffmanTree, List<char> symbols)
        {
            var codeTable = new Dictionary<char, string>();

            symbols.ForEach(key =>
            {
                var code = new StringBuilder();
                var temp = huffmanTree.Root;

                while (temp.Value.Length > 1)
                {
                    if (temp.LeftChild.Value.Contains(key))
                    {
                        code.Append("0");
                        temp = temp.LeftChild;
                    }
                    else
                    {
                        code.Append("1");
                        temp = temp.RightChild;
                    }
                }

                codeTable.Add(key, code.ToString());
            });

            return codeTable;
        }

        /// <summary>
        /// Кодирование файла.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="bufferSize"></param>
        public static void EncodeFile(ComplexMatrix matrix, int bufferSize = 4)
        {
            WriteToABinaryFile(matrix);

            var symbolsTable = CreateProbabilityTable();
            var huffmanTree = CreateHuffmanTree(symbolsTable);
            var codeTable = CreateCodeTable(huffmanTree, symbolsTable.Keys.ToList());

            using var reader = new StreamReader(File.Open(UncodedImageFile, FileMode.OpenOrCreate), Encoding.Default);
            using var writer = File.Create(EncodedImageFile);

            // Сохраняем дерево.
            var saveTree = new BinaryFormatter();
            saveTree.Serialize(writer, huffmanTree);

            var binaryCode = new StringBuilder();
            int temp;
            while ((temp = reader.Read()) > -1)
            {
                var symbol = Convert.ToChar(temp);
                codeTable.TryGetValue(symbol, out var code);
                binaryCode.Append(code);
                if (binaryCode.Length > bufferSize * 8)
                {
                    var bytes = StringToByte(binaryCode.ToString().Substring(0, bufferSize * 8));
                    binaryCode.Remove(0, bufferSize * 8);
                    foreach (var i in bytes)
                        writer.WriteByte(i);
                }
            }

            if (binaryCode.Length != 0)
            {
                var bytes = StringToByte(binaryCode.ToString().PadRight(bufferSize * 8, '0'));
                foreach (var i in bytes)
                    writer.WriteByte(i);
            }
        }

        /// <summary>
        /// Декодирование файла, закодированный с помощью алгоритма Хаффмана.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="readBufferSize"></param>
        /// <param name="writeBuffersize"></param>
        public static void DecodeFile(BinaryReader reader, int readBufferSize = 8, int writeBuffersize = 10)
        {
            using var writer = File.Create(DecodedImageFile);

            // Загружаем сохраненное дерево Хаффмана.
            var saveTree = new BinaryFormatter();
            var huffmanTree = (BinaryTree<string>)saveTree.Deserialize(reader.BaseStream);

            var decodedStr = new StringBuilder();
            var binaryCode = new StringBuilder();

            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                var buffer = reader.ReadBytes(readBufferSize);
                foreach (var i in buffer)
                    binaryCode.Append(Convert.ToString(i, 2).PadLeft(8, '0'));

                var temp = binaryCode.ToString();

                while (temp.Length >= readBufferSize)
                {
                    try
                    {
                        decodedStr.Append(DecodeSymbol(ref temp, huffmanTree));
                        binaryCode = new StringBuilder(temp);
                    }
                    catch (Exception)
                    {
                        break;
                    }

                    if (decodedStr.Length >= writeBuffersize)
                    {
                        var writeBuffer = Encoding.Default.GetBytes(decodedStr.ToString());
                        writer.Write(writeBuffer, 0, writeBuffer.Length);
                        decodedStr.Remove(0, writeBuffersize);
                    }
                }
            }

            var writeBuffer2 = Encoding.Default.GetBytes(decodedStr.ToString());
            writer.Write(writeBuffer2, 0, writeBuffer2.Length);
        }

        /// <summary>
        /// Декодирование одного символа с помощью данного дерева Хаффмана.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="huffmanTree"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        private static char DecodeSymbol(ref string code, BinaryTree<string> huffmanTree)
        {
            var node = huffmanTree.Root;
            var count = 0;
            foreach (var i in code)
            {
                if (node.Value.Length == 1)
                {
                    code = code.Remove(0, count);
                    return node.Value.ElementAt(0);
                }

                if (i == '0')
                {
                    node = node.LeftChild;
                    count++;
                }
                else
                {
                    node = node.RightChild;
                    count++;
                }
            }

            throw new FileNotFoundException();
        }

        public static ComplexMatrix GetDecodeMatrix()
        {
            using var reader = new StreamReader(File.OpenRead(DecodedImageFile), Encoding.Default);

            var data = reader.ReadToEnd().Split(' ');
            var intEnumerator = data.GetEnumerator();
            intEnumerator.MoveNext();

            var width = Convert.ToInt32(intEnumerator.Current);
            intEnumerator.MoveNext();
            var height = Convert.ToInt32(intEnumerator.Current);
            intEnumerator.MoveNext();

            var matrix = new ComplexMatrix(width, height, true);
            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
            {
                matrix.Matrix[i][j] = new Complex(Convert.ToInt32(intEnumerator.Current), 0);
                intEnumerator.MoveNext();
            }

            return matrix;
        }

        /// <summary>
        /// Преобразовывает строку
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static byte[] StringToByte(string str)
        {
            var bytes = new byte[str.Length / 8];
            for (var i = 0; i < str.Length / 8; i++)
                bytes[i] = Convert.ToByte(str.Substring(i * 8, 8), 2);
            return bytes;
        }
    }
}