import sys, io, re
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8', errors='replace')

path = 'D:/thayhai/banhang/nopcommerce_fixed.sql'
with open(path, 'r', encoding='utf-8') as f:
    lines = f.readlines()

# Find ActivityLog ON in fixed file
for i, line in enumerate(lines):
    if '[ActivityLog]' in line and 'IDENTITY_INSERT' in line and 'ON' in line:
        print(f"ActivityLog ON at line {i+1}")
        # Show next 20 lines
        for j in range(i, min(i+20, len(lines))):
            print(f"  {j+1}: {repr(lines[j].rstrip()[:150])}")
        break

# Find ActivityLog OFF
print()
for i, line in enumerate(lines):
    if '[ActivityLog]' in line and 'IDENTITY_INSERT' in line and 'OFF' in line:
        print(f"ActivityLog OFF at line {i+1}")
        # Show surrounding lines
        for j in range(max(0,i-3), min(i+5, len(lines))):
            print(f"  {j+1}: {repr(lines[j].rstrip()[:150])}")
        break

# Find GOs around the ActivityLog section
print()
for i, line in enumerate(lines):
    if '[ActivityLog]' in line and 'IDENTITY_INSERT' in line and 'ON' in line:
        al_on = i
        break

al_on_line = al_on
# Find next few GOs
go_count = 0
for i in range(al_on_line, len(lines)):
    if lines[i].strip() == 'GO':
        go_count += 1
        print(f"GO at line {i+1} (GO #{go_count} after ActivityLog ON)")
        if go_count >= 5:
            break

# Check if any ActivityLog INSERT spans multiple lines
print("\nChecking for multi-line ActivityLog inserts...")
in_actlog = False
for i, line in enumerate(lines):
    if '[ActivityLog]' in line and 'IDENTITY_INSERT' in line and 'ON' in line:
        in_actlog = True
        continue
    if '[ActivityLog]' in line and 'IDENTITY_INSERT' in line and 'OFF' in line:
        in_actlog = False
        break
    if in_actlog and 'INSERT' in line and '[ActivityLog]' in line:
        stripped = line.rstrip()
        if not stripped.endswith(')'):
            print(f"  Multi-line INSERT at line {i+1}: ...{repr(stripped[-60:])}")
