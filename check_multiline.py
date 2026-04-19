import sys, io, re
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8', errors='replace')

path = 'D:/thayhai/banhang/nopcommerce_fixed.sql'
with open(path, 'r', encoding='utf-8') as f:
    lines = f.readlines()

# Show lines 4504-4512 in full
print("Lines 4504-4512 (full):")
for i in range(4503, 4512):
    print(f"  {i+1}: {repr(lines[i].rstrip())}")

# Also show lines 4529-4542 full
print("\nLines 4529-4542 (full):")
for i in range(4528, 4542):
    print(f"  {i+1}: {repr(lines[i].rstrip())}")

# Find ALL non-INSERT, non-SET, non-PRINT, non-GO, non-empty lines in ActivityLog batch
print("\nSuspicious lines in ActivityLog batch (4503-4689):")
for i in range(4503, 4689):
    line = lines[i]
    stripped = line.strip()
    if stripped and not stripped.startswith('INSERT') and not stripped.startswith('SET') \
       and not stripped.startswith('PRINT') and stripped != 'GO' and not stripped.startswith('--'):
        print(f"  {i+1}: {repr(stripped[:150])}")
