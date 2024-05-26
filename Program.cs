﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestCodeGeneration
{
	class Program
	{
		static void Main(string[] args)
		{
			List<string> compositionOrder = new List<string>();
			compositionOrder.Add("ManageStoreCRUDService::createStore");
			compositionOrder.Add("CoCoMESystem::openStore");
			compositionOrder.Add("ManageCashDeskCRUDService::createCashDesk");
			compositionOrder.Add("CoCoMESystem::openCashDesk");

			string code = Generate(compositionOrder, "python");
			Console.WriteLine(code);
			using (StreamWriter sw = new StreamWriter("program.py"))
				sw.WriteLine(code);

		}

		static string ExtractParenthesesContent(string input)
		{
			// 提取括号及其内容的函数
			// 查找第一个左括号的索引
			int leftParenthesesIndex = input.IndexOf('(');
			if (leftParenthesesIndex == -1)
			{
				throw new Exception("Left parentheses not found.");
			}

			// 查找第一个右括号的索引
			int rightParenthesesIndex = input.IndexOf(')');
			if (rightParenthesesIndex == -1)
			{
				throw new Exception("Right parentheses not found.");
			}

			// 提取括号及其内容
			string content = input.Substring(leftParenthesesIndex, rightParenthesesIndex - leftParenthesesIndex + 1);
			return content;
		}

		static List<string> QueryParameters(string className, string functionName)
		{
			//查询 remodel， 取出参数的类型的列表
			string path = @"C:\Users\p2215981\Desktop\Liu.Lixue\coroutine-program\RequirementAnalysisTests\cocome.remodel";
			var reModelContent = File.ReadAllText(path);
			string[] lines = reModelContent.Split("\n");

			List<string> parameterTypes = new List<string>();
			for (int i = 0; i < lines.Length; i++)
			{
				if (lines[i].Contains(className + "::" + functionName))
				{
					string contentLine = ExtractParenthesesContent(lines[i]);

					var parameterEnum = contentLine.Split(",");
					for (int j = 0; j < parameterEnum.Length; j++)
					{
						parameterTypes.Add(parameterEnum[j].Split(":")[1].Trim(' ', '(', ')'));
					}

					return parameterTypes;
				}
			}
			//如果前面找到了，直接走上面的return，这里需要输出在remodel文件中找不到该函数的状态。
			throw new ArgumentException($"remodel file donnot contain this function: {className + "::" + functionName}");

		}

		static string GetValueOfType(string type)
		{
			switch (type)
			{
				case "String":
					return "\"a\"";
				case "Integer":
					return "1";
				case "Boolean":
					return "False";
				default:
					throw new NotSupportedException($"We don't know how to generate a value of type {type}.");
			}
		}


		static string GenerateFunctionCall(string className, string functionName)
		{
			string code = functionName + "(";
			var parameters = QueryParameters(className, functionName);

			code += string.Join(", ", parameters.Select(parameterType => GetValueOfType(parameterType)));

			return code + ")\n";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="compositionOrder"></param>
		/// <returns>符合 targetLanguage的代码 </returns>
		public static string Generate(List<string> compositionOrder, string targetLanguage)
		{

			if (targetLanguage == "python")
			{
				string code = "";
				foreach (var function in compositionOrder)
				{
					var parts = function.Split("::");
					string className = parts[0];
					string functionName = parts[1];
					code += GenerateFunctionCall(className, functionName);
				}
				return code;
			}
			else if (targetLanguage == "cpp")
			{

				throw new NotSupportedException($"Generating code in {targetLanguage} is out of scope of this paper.");
			}
			else
				throw new NotSupportedException($"Generating code in {targetLanguage} is out of scope of this paper.");
		}
	}
}
