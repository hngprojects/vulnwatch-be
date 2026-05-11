package com.vulnwatch.worker.scanners.dns.rules;

import com.vulnwatch.worker.scanners.dns.models.Finding;
import com.vulnwatch.worker.scanners.dns.models.ScanContext;
import org.xbill.DNS.TXTRecord;

import java.util.ArrayList;
import java.util.List;

public class TxtLeakageRules implements Rule {
    @Override
    public List<Finding> evaluate(ScanContext context) {
        List<Finding> findings = new ArrayList<>();

        for (TXTRecord r : context.txtRecordList()) {
            String txt = String.join("", r.getStrings());


            if (txt.contains("verify") || txt.contains("verification")) {
                findings.add(Finding.low(
                        "TXT_VERIFICATION_TOKEN",
                        "Verification token exposed in DNS"
                ));
            }

            if (txt.contains("staging") || txt.contains("dev") || txt.contains("test")) {
                findings.add(Finding.medium(
                        "TXT_STAGING_REFERENCE",
                        "Staging or test infrastructure referenced in DNS"
                ));
            }

            if (txt.matches(".*[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,}.*")) {
                findings.add(Finding.medium(
                        "TXT_EMAIL_EXPOSURE",
                        "Email address exposed in TXT records"
                ));
            }

            // internal hostnames
            if (txt.contains("internal") || txt.contains("corp") || txt.contains("intranet")) {
                findings.add(Finding.medium(
                        "TXT_INTERNAL_REFERENCE",
                        "Internal infrastructure reference exposed"
                ));
            }
        }

        return findings;
    }
}
