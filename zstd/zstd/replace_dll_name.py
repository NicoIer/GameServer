import os

if __name__ == "__main__":
    generated_cs_file_dir = "./generated"
    origin_attribute_header = "[DllImport(\"libzstd\", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]"
    target_attribute_header = "[DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]"
    
    for filename in os.listdir(generated_cs_file_dir):
        if filename.endswith(".cs"):
            file_path = os.path.join(generated_cs_file_dir, filename)
            with open(file_path, "r", encoding="utf-8") as file:
                content = file.read()
    
            modified_content = content.replace(origin_attribute_header, target_attribute_header)
    
            with open(file_path, "w", encoding="utf-8") as file:
                file.write(modified_content)
    
            print(f"Processed file: {file_path}")