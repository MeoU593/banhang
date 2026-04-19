import sys, io, re
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8', errors='replace')

path = 'D:/thayhai/banhang/nopcommerce_fixed.sql'
with open(path, 'r', encoding='utf-8') as f:
    lines = f.readlines()

# Find ScheduleTask inserts
for i, line in enumerate(lines):
    if '[ScheduleTask]' in line and 'IDENTITY_INSERT' in line and 'ON' in line:
        print(f"ScheduleTask section at line {i+1}")
        for j in range(i, min(i+6, len(lines))):
            print(f"  {j+1}: {lines[j].rstrip()[:200]}")
        break

# Check IDENTITY_INSERT ON/OFF balance
on_counts = {}
off_counts = {}
for line in lines:
    m = re.search(r'SET IDENTITY_INSERT \[dbo\]\.\[(\w+)\] (ON|OFF)', line)
    if m:
        tbl, state = m.group(1), m.group(2)
        if state == 'ON':
            on_counts[tbl] = on_counts.get(tbl, 0) + 1
        else:
            off_counts[tbl] = off_counts.get(tbl, 0) + 1

mismatches = [(t, on_counts.get(t,0), off_counts.get(t,0))
              for t in set(list(on_counts)+list(off_counts))
              if on_counts.get(t,0) != off_counts.get(t,0)]
if mismatches:
    print(f"\nMismatched ON/OFF: {mismatches}")
else:
    print(f"\nAll {len(on_counts)} IDENTITY_INSERT ON/OFF are balanced OK")

# Count tables in format 2
fmt2_tables = set()
for line in lines:
    m = re.search(r'SET IDENTITY_INSERT \[dbo\]\.\[(\w+)\] ON', line)
    if m:
        fmt2_tables.add(m.group(1))
print(f"Tables with data: {len(fmt2_tables)}")
print(f"Tables: {sorted(fmt2_tables)}")
