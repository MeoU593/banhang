import sys, io, re
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8', errors='replace')

path = 'D:/thayhai/banhang/nopcommerce_fixed.sql'
with open(path, 'r', encoding='utf-8') as f:
    lines = f.readlines()

# Find INSERT lines that don't end with )
truncated = []
for i, line in enumerate(lines):
    stripped = line.rstrip()
    if stripped.startswith('INSERT ') and not stripped.endswith(')'):
        truncated.append((i+1, stripped))

print(f"Total truncated INSERT lines: {len(truncated)}")
print("\nSample of truncated lines:")
for lineno, line in truncated[:20]:
    print(f"  Line {lineno}: ...{repr(line[-80:])}")

# Group by table
table_counts = {}
for lineno, line in truncated:
    m = re.search(r'INSERT \[dbo\]\.\[(\w+)\]', line)
    if m:
        tbl = m.group(1)
        table_counts[tbl] = table_counts.get(tbl, 0) + 1

print(f"\nTruncated rows by table:")
for tbl, count in sorted(table_counts.items()):
    print(f"  {tbl}: {count}")
