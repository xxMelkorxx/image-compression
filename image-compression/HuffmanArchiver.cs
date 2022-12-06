using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

		/// <summary>
		/// Таблица: число - вероятность числа.
		/// </summary>
		private Dictionary<int, double> _numberTable;

		/// <summary>
		/// Закодированная таблица: число - битовый код числа.
		/// </summary>
		private Dictionary<int, string> _codeTable;

		/// <summary>
		/// Дерево Хаффмана.
		/// </summary>
		private BinaryTree<string> _huffmanTree;

		private const string UncodedImageFile = "uncodedImage.binary";
		private const string EncodedImageFile = "encodedImage.binary";
		private const string DecodedImageFile = "decodedImage.binary";
		private Encoding _encoding = Encoding.Default;

		public HuffmanArchiver(ComplexMatrix matrix)
		{
			WriteToABinaryFile(matrix);
		}

		private void WriteToABinaryFile(ComplexMatrix matrix)
		{
			using var writer = new BinaryWriter(File.Open(UncodedImageFile, FileMode.Create), _encoding);
			//writer.Write(matrix.Width);
			//writer.Write(matrix.Height);
			for (var i = 0; i < matrix.Width; i++)
				for (var j = 0; j < matrix.Height; j++)
					writer.Write((int)matrix.Matrix[i][j].Real);
		}

		/// <summary>
		/// Создание таблицы: символ - вероятность
		/// </summary>
		/// <returns></returns>
		private Dictionary<int, double> CreateProbabilityTable()
		{
			if (!File.Exists(UncodedImageFile))
				throw new FileNotFoundException();

			var symbolCount = 0;

			// Первичная таблица: число - количество чисел.
			var first = new Dictionary<int, int>();

			// Вторичная таблица: int - вероятность числа.
			var second = new Dictionary<int, double>();

			// Чтение файла.
			using var reader = new BinaryReader(File.Open(UncodedImageFile, FileMode.Open), _encoding);
			while (reader.PeekChar() > -1)
			{
				var temp = reader.ReadInt32();
				symbolCount++;
				if (!first.ContainsKey(temp))
					first.Add(temp, 1);
				else first[temp]++;
			}

			foreach (var i in first)
				second.Add(i.Key, (double)i.Value / symbolCount);

			return second;
		}

		/// <summary>
		/// Создаёт бинарное дерево Хаффмана
		/// </summary>
		/// <returns></returns>
		private BinaryTree<string> CreateHuffmanTree()
		{
			var binaryTree = new Dictionary<string, double>();
			foreach (var i in _numberTable)
				binaryTree.Add(Convert.ToString(i.Key, 2).PadLeft(32, '0'), i.Value);

			var nodeList = new Dictionary<string, BinaryTreeNode<string>>();
			foreach (var i in binaryTree.Keys)
				nodeList.Add(i, new BinaryTreeNode<string>(i));

			while (binaryTree.Count > 1)
			{
				// Отбирает пару ключей с наименьшей вероятностью.
				var helpmass = binaryTree.OrderBy(pair => pair.Value).Take(2).ToArray();
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
		/// Создание кодовой таблицы
		/// </summary>
		/// <returns></returns>
		private Dictionary<int, string> CreateCodeTable()
		{
			var codeTable = new Dictionary<int, string>();

			_numberTable.Keys.ToList().ForEach(key =>
			{
				var code = new StringBuilder();
				var temp = _huffmanTree.Root;

				while (temp.Value.Length > 32)
				{
					if (temp.LeftChild.Value.Contains(Convert.ToString(key, 2).PadLeft(32, '0')))
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
		/// <param name="bufferSize"></param>
		public void EncodeFile(int bufferSize = 4)
		{
			if (!File.Exists(UncodedImageFile))
				throw new FileNotFoundException();

			_numberTable = CreateProbabilityTable();
			_huffmanTree = CreateHuffmanTree();
			_codeTable = CreateCodeTable();

			using var reader = new BinaryReader(File.Open(UncodedImageFile, FileMode.Open), _encoding);
			using var writer = new BinaryWriter(File.Open(EncodedImageFile, FileMode.Create), _encoding);

			// Сохраняем дерево.
			var saveTree = new BinaryFormatter();
			saveTree.Serialize(writer.BaseStream, _huffmanTree);

			var binaryCode = new StringBuilder();
			while (reader.PeekChar() > -1)
			{
				var temp = reader.ReadInt32();
				_codeTable.TryGetValue(temp, out var code);
				binaryCode.Append(code);
				if (binaryCode.Length > bufferSize * 32)
				{
					var ints = StringToInts(binaryCode.ToString().Substring(0, bufferSize * 32));
					binaryCode.Remove(0, bufferSize * 32);
					foreach (var i in ints)
						writer.Write(i);
				}
			}

			if (binaryCode.Length != 0)
			{
				var ints = StringToInts(binaryCode.ToString().PadRight(bufferSize * 32, '0'));
				foreach (var i in ints)
					writer.Write(i);
			}
		}

		/// <summary>
		/// Декодирование файла, закодированный с помощью алгоритма Хаффмана.
		/// </summary>
		/// <param name="readBufferSize"></param>
		public void DecodeFile(int readBufferSize = 8)
		{
			if (!File.Exists(EncodedImageFile))
				throw new FileNotFoundException();

			var decodedStr = new StringBuilder();
			var binaryCode = new StringBuilder();

			using var reader = new BinaryReader(File.Open(EncodedImageFile, FileMode.Open), _encoding);
			using var writer = new BinaryWriter(File.Open(DecodedImageFile, FileMode.Create), _encoding);

			// Загружаем сохраненное дерево Хаффмана.
			var saveTree = new BinaryFormatter();
			_huffmanTree = (BinaryTree<string>)saveTree.Deserialize(reader.BaseStream);

			while (reader.PeekChar() > -1)
			{
				var buffer = new List<int>();
				for (var i = 0; i < readBufferSize; i++)
					if (reader.BaseStream.Position < reader.BaseStream.Length)
						buffer.Add(reader.ReadInt32());

				foreach (var i in buffer)
					binaryCode.Append(Convert.ToString(i, 2).PadLeft(32, '0'));

				var temp = binaryCode.ToString();
				while (temp.Length >= readBufferSize)
				{
					decodedStr.Append(DecodeSymbol(ref temp));
					if (decodedStr.Length == 0) break;
					binaryCode = new StringBuilder(temp);

					var writeBuffer = Convert.ToInt32(decodedStr.ToString(), 2);
					writer.Write(writeBuffer);
					decodedStr.Remove(0, 32);
				}
			}
		}

		/// <summary>
		/// Декодирование одного символа с помощью данного дерева Хаффмана
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		private string DecodeSymbol(ref string code)
		{
			var node = _huffmanTree.Root;
			var count = 0;
			foreach (var i in code)
			{
				if (node.Value.Length == 32)
				{
					code = code.Remove(0, count);
					return node.Value;
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

			return string.Empty;
		}

		/// <summary>
		/// Преобразовывает строку
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		private static int[] StringToInts(string str)
		{
			var ints = new int[str.Length / 32];

			for (var i = 0; i < str.Length / 32; i++)
				ints[i] = Convert.ToInt32(str.Substring(i * 32, 32), 2);

			return ints;
		}
	}
}